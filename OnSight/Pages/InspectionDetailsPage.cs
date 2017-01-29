using System;

using Xamarin.Forms;

namespace OnSight
{
	public class InspectionDetailsPage : ContentPage
	{
		#region Constant Fields
		readonly InspectionDetailsViewModel _viewModel;
		readonly int _inspectionId;
		Button _viewNotesButton, _viewPhotosButton;
		#endregion

		#region Constructors
		public InspectionDetailsPage(int inspectionId)
		{
			_inspectionId = inspectionId;

			_viewModel = new InspectionDetailsViewModel(inspectionId);
			BindingContext = _viewModel;

			var titleEntry = new Entry
			{
				Placeholder = "Title"
			};
			titleEntry.SetBinding(Entry.TextProperty, nameof(_viewModel.TitleText));

			_viewNotesButton = new Button
			{
				Text = "Notes"
			};

			_viewPhotosButton = new Button
			{
				Text = "Photos"
			};

			this.SetBinding(TitleProperty, nameof(_viewModel.TitleText));

			Padding = new Thickness(20, 10);

			var stackLayout = new StackLayout
			{
				Children = {
					titleEntry,
					_viewNotesButton,
					_viewPhotosButton
				}
			};

			Content = new ScrollView
			{
				Content = stackLayout
			};
		}
		#endregion

		#region Methods

		protected override void OnAppearing()
		{
			base.OnAppearing();

			_viewNotesButton.Clicked += HandleViewNotesButtonClicked;
			_viewPhotosButton.Clicked += HandleViewPhotosButtonClicked;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel?.SaveDataCommand?.Execute(null);

			_viewNotesButton.Clicked -= HandleViewNotesButtonClicked;
			_viewPhotosButton.Clicked -= HandleViewPhotosButtonClicked;
		}
		#endregion

		void HandleViewNotesButtonClicked(object sender, EventArgs e)
		{
			//ToDo
		}

		void HandleViewPhotosButtonClicked(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new PhotosListPage(_inspectionId)));
		}
	}
}
