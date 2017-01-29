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
		//protected override void OnPropertyChanged(string propertyName = null)
		//{
		//	base.OnPropertyChanged(propertyName);

		//	Text = string.Empty;
		//	Detail = string.Empty;
		//	//ImageSource = null;

		//	var photoModel = BindingContext as PhotoModel;

		//	Text = photoModel?.ImageName;
		//	ImageSource = photoModel?.ImageSource;
		//}
	}
}
