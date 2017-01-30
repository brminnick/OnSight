using System;

using Xamarin.Forms;

namespace OnSight
{
	public class HSBViewCell : ViewCell
	{
		#region Fields
		Image _munichIcon;
		Label _titleLabel;
		Label _titleTextLabel;
		#endregion

		#region Constructors
		public HSBViewCell()
		{
			_munichIcon = new Image();

			_titleLabel = new Label
			{
				FontAttributes = FontAttributes.Bold,
				Text = "Title"
			};

			_titleTextLabel = new Label();

			var gridLayout = new Grid
			{
				Padding = new Thickness(2),
				RowSpacing = 2,
				ColumnSpacing = 10,

				RowDefinitions = {
					new RowDefinition{ Height = new GridLength (45, GridUnitType.Absolute) },
				},
				ColumnDefinitions = {
					new ColumnDefinition{ Width = new GridLength (45 , GridUnitType.Absolute) },
					new ColumnDefinition{ Width = new GridLength (1, GridUnitType.Star) }
				}
			};

			var labelStack = new StackLayout
			{
				Children = {
					_titleLabel,
					_titleTextLabel
				}
			};

			gridLayout.Children.Add(_munichIcon, 0, 0);
			gridLayout.Children.Add(labelStack, 1, 0);

			View = gridLayout;
		}
		#endregion

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			var item = BindingContext as InspectionModel;

			_titleTextLabel.Text = item?.InspectionTitle;

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
				case Device.Android:
					_munichIcon.Source = "MunichREIcon";
					break;
				case Device.Windows:
					_munichIcon.Source = "Assets/MunichREIcon.png";
					break;
				default:
					throw new Exception("Runtime Platform Not Supported");
			}
		}
	}
}
