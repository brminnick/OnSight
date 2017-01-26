using Xamarin.Forms;

namespace OnSight
{
	public class HSBImageCell : ImageCell
	{
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			Detail = null;

			var item = BindingContext as InspectionModel;

			Detail = item.InspectionTitle;
			ImageSource = "HSBLogo";
		}
	}
}
