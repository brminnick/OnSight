using System;
using Xamarin.Forms;
namespace OnSight
{
	public class InspectionDetailsPage : BaseContentPage<InspectionDetailsViewModel>
	{
		#region Constructors
		public InspectionDetailsPage()
		{
			var titleLabel = new Label
			{
				Text = "Title"
			};

			var titleEntry = new Entry
			{
				Placeholder = "Title"
			};
			titleEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.TitleText));

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
