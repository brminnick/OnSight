using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.CommunityToolkit.Markup;
using static Xamarin.CommunityToolkit.Markup.GridRowsColumns;
using static OnSight.MarkupExtensions;

namespace OnSight
{
    class InspectionDetailsPage : BaseContentPage<InspectionDetailsViewModel>
    {
        readonly string _inspectionId;

        public InspectionDetailsPage(string inspectionId) : base(new InspectionDetailsViewModel(inspectionId))
        {
            _inspectionId = inspectionId;

            NavigationPage.SetBackButtonTitle(this, "");

            this.SetBinding(TitleProperty, nameof(InspectionDetailsViewModel.TitleText));

            Padding = new Thickness(20, 10);

            Content = new Grid
            {
                RowDefinitions = Rows.Define(
                    (Row.Title, AbsoluteGridLength(30)),
                    (Row.Notes, AbsoluteGridLength(300)),
                    (Row.Button, Star)),

                Children =
                {
                    new Entry { Placeholder = "Title", HorizontalTextAlignment = TextAlignment.Center }
                        .Row(Row.Title)
                        .Bind(Entry.TextProperty, nameof(InspectionDetailsViewModel.TitleText)),

                    new Editor { BackgroundColor = ColorConstants.EditorBackgroundColor, HeightRequest = 200 }
                        .Row(Row.Notes)
                        .Bind(Editor.TextProperty, nameof(InspectionDetailsViewModel.NotesText)),

                    new Button { Text = "Photos" }.Top()
                        .Row(Row.Button)
                        .Invoke(button=>button.Clicked += HandleViewPhotosButtonClicked)
                }
            };
        }

        enum Row { Title, Notes, Button }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModel.SaveDataCommand.Execute(null);
        }

        void HandleViewPhotosButtonClicked(object sender, EventArgs e) =>
            MainThread.BeginInvokeOnMainThread(() => Navigation.PushAsync(new PhotosListPage(_inspectionId)));
    }
}
