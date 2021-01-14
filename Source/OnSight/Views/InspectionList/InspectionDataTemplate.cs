using System;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;
using static OnSight.MarkupExtensions;
using static Xamarin.CommunityToolkit.Markup.GridRowsColumns;

namespace OnSight
{
    class InspectionDataTemplate : DataTemplate
    {
        public InspectionDataTemplate() : base(() => CreateDataTemplate())
        {
        }

        static Grid CreateDataTemplate() => new()
        {
            Padding = new Thickness(2),
            RowSpacing = 2,
            ColumnSpacing = 10,

            RowDefinitions = Rows.Define(
                    (Row.Title, AbsoluteGridLength(30)),
                    (Row.Description, AbsoluteGridLength(30))),

            ColumnDefinitions = Columns.Define(
                    (Column.Image, AbsoluteGridLength(60)),
                    (Column.Text, Star)),

            Children =
            {
                new Image
                {
                    Source = Device.RuntimePlatform switch
                    {
                        Device.iOS => "UniversalLeafLogo",
                        Device.Android => "UniversalLeafLogo",
                        Device.UWP => "Assets/UniversalLeafLogo.png",
                        _ => throw new NotSupportedException($"Runtime Platform, {Device.RuntimePlatform}, Not Supported")
                    }
                }.RowSpan(All<Row>()).Column(Column.Image),

                new Label().Font(bold: true).TextCenterVertical()
                    .Row(Row.Title).Column(Column.Text)
                    .Bind(Label.TextProperty,nameof(InspectionModel.InspectionTitle)),

                new Label().TextCenterVertical()
                    .Row(Row.Description).Column(Column.Text)
                    .Bind<Label, DateTimeOffset, string>(Label.TextProperty, nameof(InspectionModel.InspectionDateUTC), convert: inspectionDate => inspectionDate.ToString("MMMMM dd yyyy"))

            }
        };

        enum Row { Title, Description }
        enum Column { Image, Text }
    }
}