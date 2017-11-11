using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

using Xamarin.Forms;

using Plugin.Media;
using Plugin.Media.Abstractions;

namespace OnSight
{
    public class AddPhotoViewModel : BaseViewModel
    {
        #region Constant Fields
        readonly string _inspectionId;
        #endregion

        #region Fields
        string _photoNameText;
        bool _isAnalyzingPhoto;
        Command _takePhotoButtonCommand, _saveButtonCommand;
        ImageSource _photoImageSource;
        MediaFile _photoMediaFile;
        #endregion

        #region Constructors
        public AddPhotoViewModel(string inspectionId)
        {
            _inspectionId = inspectionId;

            Task.Run(async () => PhotoImageNameText = await GenerateDefaultPhotoName());
        }
        #endregion

        #region Properties
        public Command SaveButtonCommand => _saveButtonCommand ??
            (_saveButtonCommand = new Command(async () => await ExecuteSaveButtonCommand()));

        public Command TakePhotoButtonCommand => _takePhotoButtonCommand ??
            (_takePhotoButtonCommand = new Command(async () => await ExecuteTakePhotoButtonCommand()));

        public ImageSource PhotoImageSource
        {
            get => _photoImageSource;
            set => SetProperty(ref _photoImageSource, value);
        }

        public string PhotoImageNameText
        {
            get => _photoNameText;
            set => SetProperty(ref _photoNameText, value);
        }

        public bool IsValidatingPhoto
        {
            get => _isAnalyzingPhoto;
            set => SetProperty(ref _isAnalyzingPhoto, value);
        }

        MediaFile PhotoMediaFile
        {
            get => _photoMediaFile;
            set => SetProperty(ref _photoMediaFile, value, async () => await UpdatePhotoImageSource());
        }
        #endregion

        #region Events
        public event EventHandler DuplicateImageNameDetected;
        public event EventHandler DisplayNoCameraAvailableAlert;
        public event EventHandler PhotoSavedToDatabaseCompleted;
        public event EventHandler<InvalidPhotoEventArgs> DisplayInvalidPhotoAlert;
        #endregion

        #region Methods
        async Task ExecuteTakePhotoButtonCommand() =>
            PhotoMediaFile = await GetMediaFileFromCamera("OnSight");

        async Task ExecuteSaveButtonCommand()
        {
            if (IsValidatingPhoto)
                return;

            var photoModelList = await PhotoModelDatabase.GetAllPhotosForInspection(_inspectionId);

            var doesPhotoImageNameTextExist = photoModelList?.FirstOrDefault(x => x.ImageName.Equals(PhotoImageNameText)) != null;

            switch (doesPhotoImageNameTextExist)
            {
                case true:
                    OnDuplicateImageNameDetected();
                    break;

                case false:
                    await SavePhotoToDatabase();
                    OnPhotoSavedToDatabaseCompleted();
                    break;
            }
        }

        async Task<MediaFile> GetMediaFileFromCamera(string directory)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                OnDisplayNoCameraAvailableAlert();
                return null;
            }

            var mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Small,
                Directory = directory,
                DefaultCamera = CameraDevice.Rear,
            });

            return mediaFile;
        }

        Task SavePhotoToDatabase()
        {
            var photoModel = new PhotoModel
            {
                InspectionModelId = _inspectionId,
                ImageName = PhotoImageNameText,
                Image = ConvertStreamToByteArray(PhotoMediaFile.GetStream())
            };

            return PhotoModelDatabase.SavePhoto(photoModel);
        }

        byte[] ConvertStreamToByteArray(Stream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        async Task<string> GenerateDefaultPhotoName()
        {
            int defaultPhotoNumber = 1;
            string defaultPhotoText = "Photo";

            var photoModelList = await PhotoModelDatabase.GetAllPhotosForInspection(_inspectionId);

            if (photoModelList != null)
            {
                foreach (PhotoModel photoModel in photoModelList)
                {
                    if (photoModel.ImageName.Equals($"{defaultPhotoText} {defaultPhotoNumber}"))
                        defaultPhotoNumber++;
                }
            }

            return $"{defaultPhotoText} {defaultPhotoNumber}";
        }

        Task UpdatePhotoImageSource()
        {
            if (PhotoMediaFile == null)
                return Task.CompletedTask;

            PhotoImageSource = ImageSource.FromStream(PhotoMediaFile.GetStream);

            return ValidatePhoto();
        }

        Stream GetPhotoStream(MediaFile mediaFile, bool disposeMediaFile)
        {
            var stream = mediaFile.GetStream();

            if (disposeMediaFile)
                mediaFile.Dispose();

            return stream;
        }

        async Task ValidatePhoto()
        {
            IsValidatingPhoto = true;

            AnalysisResult imageAnalysisResult;
            bool isImageRacyOrContainAdultContent, doesImageContainAcceptablePhotoTags, invalidAPIKey = false, internetConnectionFailed = false;

            var visionClient = new VisionServiceClient(CognitiveServicesConstants.VisionAPIKey);
            var visualFeatures = new VisualFeature[]
            {
                VisualFeature.Adult,
                VisualFeature.Description
            };

            try
            {
                imageAnalysisResult = await visionClient.AnalyzeImageAsync(GetPhotoStream(PhotoMediaFile, false), visualFeatures);
            }
            catch (ClientException e) when (e.HttpStatus.Equals(System.Net.HttpStatusCode.Unauthorized))
            {
                DebugHelpers.PrintException(e);

                imageAnalysisResult = null;
                invalidAPIKey = true;
            }
            catch (Exception e)
            {
                DebugHelpers.PrintException(e);

                imageAnalysisResult = null;
                internetConnectionFailed = true;
            }

            isImageRacyOrContainAdultContent = (imageAnalysisResult?.Adult?.IsAdultContent ?? false) || (imageAnalysisResult?.Adult.IsRacyContent ?? false);
            doesImageContainAcceptablePhotoTags = imageAnalysisResult?.Description?.Tags?.Intersect(CognitiveServicesConstants.AcceptablePhotoTags)?.Any() ?? false;

            if (isImageRacyOrContainAdultContent
                || !doesImageContainAcceptablePhotoTags
                || invalidAPIKey
                || internetConnectionFailed)
            {
                OnDisplayInvalidPhotoAlert(isImageRacyOrContainAdultContent, doesImageContainAcceptablePhotoTags, invalidAPIKey, internetConnectionFailed);
                PhotoMediaFile.Dispose();
                PhotoImageSource = null;
            }

            IsValidatingPhoto = false;
        }

        void OnDisplayInvalidPhotoAlert(
            bool isImageInappropriate,
            bool doesImageContainAcceptablePhotoTags,
            bool invalidAPIKey,
            bool internetConnectionFailed) =>
        DisplayInvalidPhotoAlert?.Invoke(this, new InvalidPhotoEventArgs(isImageInappropriate, doesImageContainAcceptablePhotoTags, invalidAPIKey, internetConnectionFailed));

        void OnDisplayNoCameraAvailableAlert() =>
            DisplayNoCameraAvailableAlert?.Invoke(this, EventArgs.Empty);

        void OnDuplicateImageNameDetected() =>
            DuplicateImageNameDetected?.Invoke(this, EventArgs.Empty);

        void OnPhotoSavedToDatabaseCompleted() =>
            PhotoSavedToDatabaseCompleted?.Invoke(this, EventArgs.Empty);
        #endregion
    }
}
