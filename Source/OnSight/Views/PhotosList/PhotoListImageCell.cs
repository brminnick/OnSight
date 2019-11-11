using Xamarin.Forms;

namespace OnSight
{
    public class PhotoListImageCell : ImageCell
    {
        public PhotoListImageCell()
        {
            this.SetBinding(TextProperty, nameof(PhotoModel.ImageName));
            this.SetBinding(ImageSourceProperty, nameof(PhotoModel.ImageSource));
        }
    }
}
