using System;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;
using static OnSight.MarkupExtensions;
using static Xamarin.CommunityToolkit.Markup.GridRowsColumns;

namespace OnSight
{
    class PhotoDataTemplate : DataTemplate
    {
        public PhotoDataTemplate() : base(() => CreateDataTemplate())
        {
        }

        static Grid CreateDataTemplate() => new()
        {
            Padding = new Thickness(2),
            RowSpacing = 2,
            ColumnSpacing = 10,

            RowDefinitions = Rows.Define(AbsoluteGridLength(60)),

            ColumnDefinitions = Columns.Define(
                    (Column.Image, AbsoluteGridLength(60)),
                    (Column.Text, Star)),

            Children =
            {
                new Image()
                    .Column(Column.Image)
                    .Bind(Image.SourceProperty, nameof(PhotoModel.ImageSource)),

                new Label().Font(size: 24,bold: true).TextCenterVertical()
                    .Column(Column.Text)
                    .Bind(Label.TextProperty,nameof(PhotoModel.ImageName)),
            }
        };

        enum Column { Image, Text }
    }
}