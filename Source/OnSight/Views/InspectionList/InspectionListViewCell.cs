using System;

using Xamarin.Forms;

namespace OnSight
{
	public class InspectionListViewCell : ViewCell
	{
		#region Fields
		Image _icon;
		Label _titleLabel;
		Label _descriptionLabel;
		#endregion

		#region Constructors
		public InspectionListViewCell()
		{
			_icon = new Image();

			_titleLabel = new Label
			{
				FontAttributes = FontAttributes.Bold,
			};

			_descriptionLabel = new Label();

			var gridLayout = new Grid
			{
				Padding = new Thickness(2),
				RowSpacing = 2,
				ColumnSpacing = 10,

				RowDefinitions = {
					new RowDefinition{ Height = new GridLength(0, GridUnitType.Auto) },
					new RowDefinition{ Height = new GridLength(0, GridUnitType.Auto) }

				},
				ColumnDefinitions = {
					new ColumnDefinition{ Width = new GridLength(0, GridUnitType.Auto) },
					new ColumnDefinition{ Width = new GridLength(0, GridUnitType.Auto) }
				}
			};

			gridLayout.Children.Add(_icon, 0, 0);
			Grid.SetRowSpan(_icon, 2);

			gridLayout.Children.Add(_titleLabel, 1, 0);
			gridLayout.Children.Add(_descriptionLabel, 1, 1);

			View = gridLayout;
		}
		#endregion

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			var item = BindingContext as InspectionModel;

			_titleLabel.Text = item?.InspectionTitle;
			_descriptionLabel.Text = item?.InspectionDateUTC.Date.ToString("MMMMM dd yyyy");

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
				case Device.Android:
					_icon.Source = "UniversalLeafLogo";
					break;
				case Device.UWP:
					_icon.Source = "Assets/UniversalLeafLogo.png";
					break;
				default:
					throw new Exception("Runtime Platform Not Supported");
			}
		}
	}
}
