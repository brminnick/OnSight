using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace OnSight
{
    public class AddPhotoViewModel : BaseViewModel
    {
        readonly string _inspectionId;

        readonly AsyncAwaitBestPractices.WeakEventManager _duplicateImageNameDetectedEventManager = new();
        readonly AsyncAwaitBestPractices.WeakEventManager _displayNoCameraAvailableAlertEventManager = new();
        readonly AsyncAwaitBestPractices.WeakEventManager _photoSavedToDatabaseCompletedEventManager = new();

        bool _isAnalyzingPhoto;
        string _photoNameText = string.Empty;
        ICommand? _takePhotoButtonCommand, _saveButtonCommand;
        ImageSource? _photoImageSource;
        MediaFile? _photoMediaFile;

        public AddPhotoViewModel(string inspectionId)
        {
            _inspectionId = inspectionId;

            GenerateDefaultPhotoName().SafeFireAndForget();
        }

        public event EventHandler DuplicateImageNameDetected
        {
            add => _duplicateImageNameDetectedEventManager.AddEventHandler(value);
            remove => _duplicateImageNameDetectedEventManager.RemoveEventHandler(value);
        }

        public event EventHandler DisplayNoCameraAvailableAlert
        {
            add => _displayNoCameraAvailableAlertEventManager.AddEventHandler(value);
            remove => _displayNoCameraAvailableAlertEventManager.RemoveEventHandler(value);
        }

        public event EventHandler PhotoSavedToDatabaseCompleted
        {
            add => _photoSavedToDatabaseCompletedEventManager.AddEventHandler(value);
            remove => _photoSavedToDatabaseCompletedEventManager.RemoveEventHandler(value);
        }

        public ICommand SaveButtonCommand => _saveButtonCommand ??= new AsyncCommand(() => ExecuteSaveButtonCommand(_inspectionId, PhotoImageNameText, PhotoMediaFile));
        public ICommand TakePhotoButtonCommand => _takePhotoButtonCommand ??= new AsyncCommand(ExecuteTakePhotoButtonCommand);

        public ImageSource? PhotoImageSource
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

        MediaFile? PhotoMediaFile
        {
            get => _photoMediaFile;
            set => SetProperty(ref _photoMediaFile, value, async () => await UpdatePhotoImageSource(PhotoMediaFile).ConfigureAwait(false));
        }

        async Task GenerateDefaultPhotoName() =>
            PhotoImageNameText = await GenerateDefaultPhotoName(_inspectionId).ConfigureAwait(false);

        async Task ExecuteTakePhotoButtonCommand() =>
            PhotoMediaFile = await MediaService.GetMediaFileFromCamera("OnSite").ConfigureAwait(false);

        async Task ExecuteSaveButtonCommand(string inspectionId, string photoImageNameText, MediaFile ?photoMediaFile)
        {
            if (IsValidatingPhoto)
                return;

            var photoModelList = await PhotoModelDatabase.GetAllPhotosForInspection(inspectionId).ConfigureAwait(false);

            var doesPhotoImageNameTextExist = photoModelList.Any(x => x.ImageName.Equals(photoImageNameText));
            if (doesPhotoImageNameTextExist)
            {
                OnDuplicateImageNameDetected();
            }
            else
            {
                await SavePhotoToDatabase(inspectionId, photoImageNameText, photoMediaFile).ConfigureAwait(false);
                OnPhotoSavedToDatabaseCompleted();
            }
        }

        Task SavePhotoToDatabase(string inspectionId, string photoImageNameText, MediaFile? photoMediaFile)
        {
            var photoModel = new PhotoModel
            {
                InspectionModelId = inspectionId,
                ImageName = photoImageNameText,
                Image = photoMediaFile is null ? null : ConvertStreamToByteArray(photoMediaFile.GetStream())
            };

            return PhotoModelDatabase.SavePhoto(photoModel);
        }

        byte[] ConvertStreamToByteArray(Stream stream)
        {
            using MemoryStream memoryStream = new MemoryStream();

            stream.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }

        async Task<string> GenerateDefaultPhotoName(string inspectionId)
        {
            int defaultPhotoNumber = 1;
            const string defaultPhotoText = "Photo";

            var photoModelList = await PhotoModelDatabase.GetAllPhotosForInspection(inspectionId).ConfigureAwait(false);

            if (!(photoModelList is null))
            {
                foreach (var photoModel in photoModelList)
                {
                    if (photoModel.ImageName?.Equals($"{defaultPhotoText} {defaultPhotoNumber}") ?? false)
                        defaultPhotoNumber++;
                }
            }

            return $"{defaultPhotoText} {defaultPhotoNumber}";
        }

        async Task UpdatePhotoImageSource(MediaFile? photoMediaFile)
        {
            if (photoMediaFile is null)
                return;

            PhotoImageSource = ImageSource.FromStream(photoMediaFile.GetStream);

            IsValidatingPhoto = true;

            try
            {
                var isPhotoValid = await ComputerVisionService.IsPhotoValid(photoMediaFile.GetStream(), new List<string> { "plant", "flower" }).ConfigureAwait(false);
                if (!isPhotoValid)
                {
                    photoMediaFile.Dispose();
                    PhotoImageSource = null;
                }
            }
            finally
            {
                IsValidatingPhoto = false;
            }
        }

        void OnDisplayNoCameraAvailableAlert() =>
            _displayNoCameraAvailableAlertEventManager.RaiseEvent(this, EventArgs.Empty, nameof(DisplayNoCameraAvailableAlert));

        void OnDuplicateImageNameDetected() =>
            _duplicateImageNameDetectedEventManager.RaiseEvent(this, EventArgs.Empty, nameof(DuplicateImageNameDetected));

        void OnPhotoSavedToDatabaseCompleted() =>
            _photoSavedToDatabaseCompletedEventManager.RaiseEvent(this, EventArgs.Empty, nameof(PhotoSavedToDatabaseCompleted));
    }
}
