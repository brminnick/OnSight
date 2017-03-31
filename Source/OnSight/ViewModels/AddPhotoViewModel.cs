using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.ProjectOxford.Vision;

using Xamarin.Forms;

using Plugin.Media;
using Plugin.Media.Abstractions;
using Microsoft.ProjectOxford.Vision.Contract;

namespace OnSight
{
	public class AddPhotoViewModel : BaseViewModel
	{
		#region Constant Fields
		readonly int _inspectionId;
		#endregion

		#region Fields
		string _photoNameText;
		bool _isAnalyzingPhoto;
		Command _takePhotoButtonCommand, _saveButtonCommand;
		ImageSource _photoImageSource;
		MediaFile _photoMediaFile;
		#endregion

		#region Constructors
		public AddPhotoViewModel(int inspectionId)
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
			get { return _photoImageSource; }
			set { SetProperty(ref _photoImageSource, value); }
		}

		public string PhotoImageNameText
		{
			get { return _photoNameText; }
			set { SetProperty(ref _photoNameText, value); }
		}

		public bool IsValidatingPhoto
		{
			get { return _isAnalyzingPhoto; }
			set { SetProperty(ref _isAnalyzingPhoto, value); }
		}

		MediaFile PhotoMediaFile
		{
			get { return _photoMediaFile; }
			set { SetProperty(ref _photoMediaFile, value, async () => await UpdatePhotoImageSource()); }
		}
		#endregion

		#region Events
		public event EventHandler DuplicateImageNameDetected;
		public event EventHandler DisplayNoCameraAvailableAlert;
		public event EventHandler PhotoSavedToDatabaseCompleted;
		public event EventHandler<InvalidPhotoEventArgs> DisplayInvalidPhotoAlert;
		#endregion

		#region Methods
		async Task ExecuteTakePhotoButtonCommand()
		{
			PhotoMediaFile = await GetMediaFileFromCamera("OnSight");
		}

		async Task ExecuteSaveButtonCommand()
		{
			if (IsValidatingPhoto)
				return;

			var photoModelList = await InspectionModelDatabase.GetAllPhotosForInspection(_inspectionId);

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

		async Task SavePhotoToDatabase()
		{
			var photoModel = new PhotoModel
			{
				InspectionModelId = _inspectionId,
				ImageName = PhotoImageNameText,
				ImageAsBase64String = ConvertStreamToBase64String(PhotoMediaFile.GetStream())
			};

			await InspectionModelDatabase.SavePhoto(photoModel);
		}

		string ConvertStreamToBase64String(Stream stream)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				stream.CopyTo(memoryStream);
				var imageAsByteArray = memoryStream.ToArray();
				return Convert.ToBase64String(imageAsByteArray);
			}
		}

		async Task<string> GenerateDefaultPhotoName()
		{
			int defaultPhotoNumber = 1;
			string defaultPhotoText = "Photo";

			var photoModelList = await InspectionModelDatabase.GetAllPhotosForInspection(_inspectionId);

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

		async Task UpdatePhotoImageSource()
		{
			if (PhotoMediaFile == null)
				return;

			PhotoImageSource = ImageSource.FromStream(PhotoMediaFile.GetStream);

			await ValidatePhoto();
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
			bool invalidAPIKey = false, internetConnectionFailed = false, isImageRacyOrContainAdultContent, doesImageContainAcceptablePhotoTags;

			var visionClient = new VisionServiceClient(CognitiveServicesConstants.VisionAPIKey);
			var visualFeatures = new VisualFeature[]
			{
				VisualFeature.Adult,
				VisualFeature.Description,
				VisualFeature.Tags
			};

			try
			{
				imageAnalysisResult = await visionClient.AnalyzeImageAsync(GetPhotoStream(PhotoMediaFile, false), visualFeatures);
			}
			catch (Exception e)
			{
				DebugHelpers.PrintException(e);

				imageAnalysisResult = null;

				if ((e is ClientException) && ((ClientException)e).HttpStatus == System.Net.HttpStatusCode.Unauthorized)
					invalidAPIKey = true;
				else
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
			bool internetConnectionFailed
		) =>
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
