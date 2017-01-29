using System;
using System.IO;
using System.Threading.Tasks;

using Xamarin.Forms;

using Plugin.Media.Abstractions;
using Plugin.Media;
using System.Linq;

namespace OnSight
{
	public class AddPhotoViewModel : BaseViewModel
	{
		#region Constant Fields
		readonly int _inspectionId;
		#endregion

		#region Fields
		string _photoNameText;
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

		public MediaFile PhotoMediaFile
		{
			get { return _photoMediaFile; }
			set { SetProperty(ref _photoMediaFile, value, UpdatePhotoImageSource); }
		}
		#endregion

		#region Events
		public event EventHandler DisplayNoCameraAvailableAlert;
		public event EventHandler DuplicateImageNameDetected;
		public event EventHandler PhotoSavedToDatabaseCompleted;
		#endregion

		#region Methods
		async Task ExecuteTakePhotoButtonCommand()
		{
			PhotoMediaFile = await GetMediaFileFromCamera("OnSight");
		}

		async Task ExecuteSaveButtonCommand()
		{
			var inspectionModel = await InspectionModelDatabase.GetInspectionModelAsync(_inspectionId);
			var photoModelList = inspectionModel.GetAllPhotos();

			var doesPhotoImageNameTextExist = photoModelList?.FirstOrDefault(x => x.ImageName.Equals(PhotoImageNameText)) != null;

			if (doesPhotoImageNameTextExist)
			{
				OnDuplicateImageNameDetected();
			}
			else
			{
				await SavePhotoToDatabase();
				OnPhotoSavedToDatabaseCompleted();
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
				ImageName = PhotoImageNameText,
				ImageStream = PhotoMediaFile.GetStream()
			};

			var inspectionModel = await InspectionModelDatabase.GetInspectionModelAsync(_inspectionId);
			inspectionModel.SavePhoto(photoModel);

			await InspectionModelDatabase.SaveInspectionModelAsync(inspectionModel);
		}

		async Task<string> GenerateDefaultPhotoName()
		{
			int defaultPhotoNumber = 1;
			string defaultPhotoText = "Photo";

			var inspectionModel = await InspectionModelDatabase.GetInspectionModelAsync(_inspectionId);
			var photoModelList = inspectionModel?.GetAllPhotos();

			if (photoModelList != null)
			{
				foreach (PhotoModel photoModel in photoModelList)
				{
					if (photoModel.ImageName.Equals($"{defaultPhotoNumber}{defaultPhotoNumber}"))
						defaultPhotoNumber++;
				}
			}

			return $"{defaultPhotoText} {defaultPhotoNumber}";
		}

		void UpdatePhotoImageSource()
		{
			PhotoImageSource = ImageSource.FromStream(PhotoMediaFile.GetStream);
		}

		void OnDisplayNoCameraAvailableAlert()
		{
			DisplayNoCameraAvailableAlert?.Invoke(null, EventArgs.Empty);
		}

		void OnDuplicateImageNameDetected()
		{
			DuplicateImageNameDetected?.Invoke(null, EventArgs.Empty);
		}

		void OnPhotoSavedToDatabaseCompleted()
		{
			PhotoSavedToDatabaseCompleted?.Invoke(null, EventArgs.Empty);
		}
		#endregion
	}
}
