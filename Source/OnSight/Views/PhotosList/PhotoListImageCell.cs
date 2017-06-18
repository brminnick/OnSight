using Xamarin.Forms;

namespace OnSight
{
	public class PhotoListImageCell : ImageCell
	{
		public PhotoListImageCell()
		{
            var photoMoel = BindingContext as PhotoModel;

			this.SetBinding(TextProperty, nameof(photoMoel.ImageName));
			this.SetBinding(ImageSourceProperty, nameof(photoMoel.ImageSource));
		}
	}
}
