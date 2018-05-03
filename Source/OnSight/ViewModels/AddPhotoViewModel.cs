using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Plugin.Media.Abstractions;

using Xamarin.Forms;

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
		Command _takePhotoButtonCommand, _saveButtonCommand, _generateDefaultPhotoNameCommand;
		ImageSource _photoImageSource;
		MediaFile _photoMediaFile;
		#endregion

		#region Constructors
		public AddPhotoViewModel(string inspectionId)
		{
			_inspectionId = inspectionId;

			GenerateDefaultPhotoNameCommand?.Execute(null);
		}
		#endregion

		#region Properties
		public Command SaveButtonCommand => _saveButtonCommand ??
			(_saveButtonCommand = new Command(async () => await ExecuteSaveButtonCommand(_inspectionId, PhotoImageNameText, PhotoMediaFile).ConfigureAwait(false)));

		public Command TakePhotoButtonCommand => _takePhotoButtonCommand ??
			(_takePhotoButtonCommand = new Command(async () => await ExecuteTakePhotoButtonCommand().ConfigureAwait(false)));

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

		Command GenerateDefaultPhotoNameCommand => _generateDefaultPhotoNameCommand ??
			(_generateDefaultPhotoNameCommand = new Command(async () => PhotoImageNameText = await GenerateDefaultPhotoName(_inspectionId).ConfigureAwait(false)));

		MediaFile PhotoMediaFile
		{
			get => _photoMediaFile;
			set => SetProperty(ref _photoMediaFile, value, async () => await UpdatePhotoImageSource(PhotoMediaFile).ConfigureAwait(false));
		}
		#endregion

		#region Events
		public event EventHandler DuplicateImageNameDetected;
		public event EventHandler DisplayNoCameraAvailableAlert;
		public event EventHandler PhotoSavedToDatabaseCompleted;
		#endregion

		#region Methods
		async Task ExecuteTakePhotoButtonCommand() =>
			PhotoMediaFile = await MediaService.GetMediaFileFromCamera("OnSite", "Onsite").ConfigureAwait(false);

		async Task ExecuteSaveButtonCommand(string inspectionId, string photoImageNameText, MediaFile photoMediaFile)
		{
			if (IsValidatingPhoto)
				return;

			var photoModelList = await PhotoModelDatabase.GetAllPhotosForInspection(inspectionId).ConfigureAwait(false);

			var doesPhotoImageNameTextExist = !(photoModelList?.FirstOrDefault(x => x.ImageName.Equals(photoImageNameText)) is null);

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

		Task SavePhotoToDatabase(string inspectionId, string photoImageNameText, MediaFile photoMediaFile)
		{
			var photoModel = new PhotoModel
			{
				InspectionModelId = inspectionId,
				ImageName = photoImageNameText,
				Image = ConvertStreamToByteArray(photoMediaFile.GetStream())
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

		async Task UpdatePhotoImageSource(MediaFile photoMediaFile)
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
			DisplayNoCameraAvailableAlert?.Invoke(this, EventArgs.Empty);

		void OnDuplicateImageNameDetected() =>
			DuplicateImageNameDetected?.Invoke(this, EventArgs.Empty);

		void OnPhotoSavedToDatabaseCompleted() =>
			PhotoSavedToDatabaseCompleted?.Invoke(this, EventArgs.Empty);
		#endregion
	}
}
