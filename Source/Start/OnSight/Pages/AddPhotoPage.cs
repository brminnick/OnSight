using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace OnSight
{
	public class AddPhotoPage : ContentPage
	{
		#region Constant Fields
		AddPhotoViewModel _viewModel;
		readonly ToolbarItem _saveButton;
		readonly Entry _photoImageNameEntry;
		#endregion

		#region Constructors
		public AddPhotoPage(int inspectionId)
		{
			_viewModel = new AddPhotoViewModel(inspectionId);
			BindingContext = _viewModel;


		}
		#endregion

		#region Methods
		protected override void OnAppearing()
		{
			base.OnAppearing();

			_viewModel.PhotoSavedToDatabaseCompleted += HandlePhotoSavedToDatabaseCompleted;
			_viewModel.DisplayNoCameraAvailableAlert += HandleDisplayNoCameraAvailableAlert;
			_viewModel.DuplicateImageNameDetected += HandleDuplicateImageNameDetected;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.PhotoSavedToDatabaseCompleted -= HandlePhotoSavedToDatabaseCompleted;
			_viewModel.DisplayNoCameraAvailableAlert -= HandleDisplayNoCameraAvailableAlert;
			_viewModel.DuplicateImageNameDetected -= HandleDuplicateImageNameDetected;
		}

		void HandleDuplicateImageNameDetected(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(() => DisplayAlert("Error: Duplicate Photo Name", $"A Photo Entitled {_photoImageNameEntry.Text} Already Exists", "Ok"));
		}

		void HandleDisplayNoCameraAvailableAlert(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(() => DisplayAlert("Error", "Camera Unavailable", "Ok"));
		}

		void HandlePhotoSavedToDatabaseCompleted(object sender, EventArgs e)
		{
			DismissPage();
		}

		void DismissPage()
		{
			Device.BeginInvokeOnMainThread(async () => await Navigation.PopModalAsync());
		}
		#endregion
	}
}
