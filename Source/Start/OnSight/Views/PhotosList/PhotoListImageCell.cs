using Xamarin.Forms;

namespace OnSight
{
	public class PhotoListImageCell : ImageCell
	{

		public PhotoListImageCell()
		{
			var bindingContext = BindingContext as PhotoModel;

			this.SetBinding(TextProperty, nameof(bindingContext.ImageName));
			this.SetBinding(ImageSourceProperty, nameof(bindingContext.ImageSource));
		}
	}
}
