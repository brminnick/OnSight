using System;

using Xamarin.Forms;

namespace OnSight
{
	public class InspectionDetailsPage : ContentPage
	{
		#region Constant Fields
		readonly InspectionDetailsViewModel _viewModel;
		readonly int _inspectionId;
		Button _viewPhotosButton;
		#endregion

		#region Constructors
		public InspectionDetailsPage(int inspectionId)
		{
			_inspectionId = inspectionId;

			_viewModel = new InspectionDetailsViewModel(inspectionId);
			BindingContext = _viewModel;

			this.SetBinding(TitleProperty, nameof(_viewModel.TitleText));

			Padding = new Thickness(20, 10);
		}
		#endregion

		#region Methods

		protected override void OnAppearing()
		{
			base.OnAppearing();

			_viewPhotosButton.Clicked += HandleViewPhotosButtonClicked;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel?.SaveDataCommand?.Execute(null);

			_viewPhotosButton.Clicked -= HandleViewPhotosButtonClicked;
		}

		void HandleViewPhotosButtonClicked(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new PhotosListPage(_inspectionId)));
		}
		#endregion
	}
}
