using System;

using Xamarin.Forms;

namespace OnSight
{
	public class HSBImageCell : ImageCell
	{
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			var item = BindingContext as InspectionModel;

			Text = item?.InspectionTitle;

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
				case Device.Android:
					ImageSource = "MunichREIcon";
					break;
				case Device.Windows:
					ImageSource = "Assets/MunichREIcon.png";
					break;
				default:
					throw new Exception("Runtime Platform Not Supported");
			}
		}
	}
}
