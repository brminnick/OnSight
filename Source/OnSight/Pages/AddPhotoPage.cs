using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using System.Text;

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

			var validatingPhotoLabel = new Label
			{
				Text = "Validating Photo",
				HorizontalTextAlignment = TextAlignment.Center
			};
			validatingPhotoLabel.SetBinding(IsVisibleProperty, nameof(_viewModel.IsValidatingPhoto));

			var validatingPhotoActivityIndicator = new ActivityIndicator();
			validatingPhotoActivityIndicator.SetBinding(IsVisibleProperty, nameof(_viewModel.IsValidatingPhoto));
			validatingPhotoActivityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(_viewModel.IsValidatingPhoto));

			var validatingPhotoFrame = new Frame
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Content = new StackLayout
				{
					Children = {
						validatingPhotoLabel,
						validatingPhotoActivityIndicator
					}
				}
			};
			validatingPhotoFrame.SetBinding(IsVisibleProperty, nameof(_viewModel.IsValidatingPhoto));

			_saveButton = new ToolbarItem();
			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
				case Device.Android:
					_saveButton.Icon = "Save";
					break;
				case Device.Windows:
					_saveButton.Icon = "Assets/Save.png";
					break;
				default:
					throw new Exception("Runtime Platform Not Supported");
			}
			_saveButton.SetBinding(ToolbarItem.CommandProperty, nameof(_viewModel.SaveButtonCommand));
			ToolbarItems.Add(_saveButton);

			var cancelButton = new ToolbarItem();
			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
				case Device.Android:
					cancelButton.Icon = "Cancel";
					break;
				case Device.Windows:
					cancelButton.Icon = "Assets/Cancel.png";
					break;
				default:
					throw new Exception("Runtime Platform Not Supported");
			}
			cancelButton.Clicked += (sender, e) => DismissPage();
			ToolbarItems.Add(cancelButton);


			var photoImage = new Image();
			photoImage.SetBinding(Image.SourceProperty, nameof(_viewModel.PhotoImageSource));

			_photoImageNameEntry = new Entry();
			_photoImageNameEntry.SetBinding(Entry.TextProperty, nameof(_viewModel.PhotoImageNameText));

			var takePhotoButton = new Button
			{
				Text = "New Photo"
			};
			takePhotoButton.SetBinding(Button.CommandProperty, nameof(_viewModel.TakePhotoButtonCommand));

			Padding = new Thickness(40, 10);

			this.SetBinding(TitleProperty, nameof(_viewModel.PhotoImageNameText));

			var gridLayout = new Grid
			{
				ColumnDefinitions = { new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } },
				RowDefinitions = {
					new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
					new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
					new RowDefinition { Height = new GridLength(300, GridUnitType.Absolute) }
				}
			};

			gridLayout.Children.Add(_photoImageNameEntry, 0, 0);
			gridLayout.Children.Add(takePhotoButton, 0, 1);
			gridLayout.Children.Add(photoImage, 0, 2);
			gridLayout.Children.Add(validatingPhotoFrame, 0, 2);

			Content = gridLayout;
		}
		#endregion

		#region Methods
		protected override void OnAppearing()
		{
			base.OnAppearing();

			_viewModel.PhotoSavedToDatabaseCompleted += HandlePhotoSavedToDatabaseCompleted;
			_viewModel.DisplayNoCameraAvailableAlert += HandleDisplayNoCameraAvailableAlert;
			_viewModel.DuplicateImageNameDetected += HandleDuplicateImageNameDetected;
			_viewModel.DisplayInvalidPhotoAlert += HandleDisplayInvalidPhotoAlert;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.PhotoSavedToDatabaseCompleted -= HandlePhotoSavedToDatabaseCompleted;
			_viewModel.DisplayNoCameraAvailableAlert -= HandleDisplayNoCameraAvailableAlert;
			_viewModel.DuplicateImageNameDetected -= HandleDuplicateImageNameDetected;
			_viewModel.DisplayInvalidPhotoAlert -= HandleDisplayInvalidPhotoAlert;
		}

		void HandleDuplicateImageNameDetected(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(() => DisplayAlert("Error: Duplicate Photo Name", $"A Photo Entitled {_photoImageNameEntry.Text} Already Exists", "Ok"));
		}

		void HandleDisplayNoCameraAvailableAlert(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(() => DisplayAlert("Error", "Camera Unavailable", "Ok"));
		}

		void HandleDisplayInvalidPhotoAlert(object sender, InvalidPhotoEventArgs e)
		{
			var errorString = new StringBuilder();

			if (e.InvalidAPIKey)
				errorString.AppendLine("Invalid API Key");
			if (e.InternetConnectionFailed)
				errorString.AppendLine("Internet Connection Failed");
			if (!e.DoesImageContainAcceptablePhotoTags)
				errorString.AppendLine("No Plant Detected");
			if (e.IsImageInappropriate)
				errorString.AppendLine("Inaproproate Image Detected");

			Device.BeginInvokeOnMainThread(() => DisplayAlert("Error", errorString.ToString(), "Ok"));
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
