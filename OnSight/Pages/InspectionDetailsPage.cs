using System;

using Xamarin.Forms;

namespace OnSight
{
	public class InspectionDetailsPage : ContentPage
	{
		#region Constant Fields
		readonly InspectionDetailsViewModel _viewModel;
		#endregion

		#region Constructors
		public InspectionDetailsPage(int inspectionId)
		{
			_viewModel = new InspectionDetailsViewModel(inspectionId);
			BindingContext = _viewModel;

			var titleEntry = new Entry
			{
				Placeholder = "Title"
			};
			titleEntry.SetBinding(Entry.TextProperty, nameof(_viewModel.TitleText));

			var viewNotesButton = new Button
			{
				Text = "Notes"
			};

			var viewImagesButton = new Button
			{
				Text = "Images"
			};

			this.SetBinding(TitleProperty, nameof(_viewModel.TitleText));

			Padding = new Thickness(20, 10);

			var stackLayout = new StackLayout
			{
				Children = {
					titleEntry,
					viewNotesButton,
					viewImagesButton
				}
			};

			Content = new ScrollView
			{
				Content = stackLayout
			};
		}
		#endregion

		#region Methods
		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel?.SaveDataCommand?.Execute(null);
		}
		#endregion
	}
}
