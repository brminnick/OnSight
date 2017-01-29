using Xamarin.Forms;

namespace OnSight
{
	public class PhotosListTextCell : TextCell
	{
		protected override void OnPropertyChanged(string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			Text = string.Empty;
			Detail = string.Empty;

			var noteItemModel = BindingContext as NoteModel;

			Text = noteItemModel?.Title;
			Detail = noteItemModel?.Details;
		}
	}
}
