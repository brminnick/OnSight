using System;

using Xamarin.Forms;

namespace OnSight
{
    public class InspectionListViewCell : ViewCell
    {
        readonly Image _icon;
        readonly Label _titleLabel;
        readonly Label _descriptionLabel;

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

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var item = (InspectionModel)BindingContext;

            _titleLabel.Text = item.InspectionTitle;
            _descriptionLabel.Text = item.InspectionDateUTC.Date.ToString("MMMMM dd yyyy");
            _icon.Source = Device.RuntimePlatform switch
            {
                Device.iOS => "UniversalLeafLogo",
                Device.Android => "UniversalLeafLogo",
                Device.UWP => "Assets/UniversalLeafLogo.png",
                _ => throw new NotSupportedException($"Runtime Platform, {Device.RuntimePlatform}, Not Supported")
            };
        }
    }
}
