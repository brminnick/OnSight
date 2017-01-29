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

			var titleLabel = new Label
			{
				Text = "Title"
			};

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

			Content = new StackLayout
			{
				Children = {
					titleLabel,
					titleEntry,
					viewNotesButton,
					viewImagesButton
				}
			};
		}
		#endregion
	}
}
