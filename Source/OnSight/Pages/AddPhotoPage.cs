using System;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Xamarin.CommunityToolkit.Markup.GridRowsColumns;

namespace OnSight
{
    class AddPhotoPage : BaseContentPage<AddPhotoViewModel>
    {
        readonly Entry _photoImageNameEntry;

        public AddPhotoPage(string inspectionId) : base(new AddPhotoViewModel(inspectionId))
        {
            ViewModel.DuplicateImageNameDetected += HandleDuplicateImageNameDetected;
            ViewModel.PhotoSavedToDatabaseCompleted += HandlePhotoSavedToDatabaseCompleted;
            ViewModel.DisplayNoCameraAvailableAlert += HandleDisplayNoCameraAvailableAlert;
            ComputerVisionService.InvalidPhotoSubmitted += HandleInvalidPhotoSubmitted;

            Padding = new Thickness(40, 10);

            this.Bind(TitleProperty, nameof(AddPhotoViewModel.PhotoImageNameText));

            ToolbarItems.Add(new ToolbarItem
            {
                IconImageSource = Device.RuntimePlatform switch
                {
                    Device.iOS => "Save",
                    Device.Android => "Save",
                    Device.UWP => "Assets/Save.png",
                    _ => throw new NotSupportedException()
                }
            }.Bind(ToolbarItem.CommandProperty, nameof(AddPhotoViewModel.SaveButtonCommand)));

            ToolbarItems.Add(new ToolbarItem
            {
                IconImageSource = Device.RuntimePlatform switch
                {
                    Device.iOS => "Cancel",
                    Device.Android => "Cancel",
                    Device.UWP => "Assets/Cancel.png",
                    _ => throw new NotSupportedException()
                }
            }.Invoke(cancelButton => cancelButton.Clicked += HandleCancelButtonClicked));

            Content = new Grid
            {
                RowDefinitions = Rows.Define(
                    (Row.Title, Auto),
                    (Row.TakePhotoButton, Auto),
                    (Row.Photo, Star)),

                Children =
                {
                    new Entry().Row(Row.Title).Assign(out _photoImageNameEntry)
                        .Bind(Entry.TextProperty, nameof(AddPhotoViewModel.PhotoImageNameText)),

                    new Button { Text = "New Plant Photo" }.Row(Row.TakePhotoButton)
                        .Bind(Button.CommandProperty, nameof(AddPhotoViewModel.TakePhotoButtonCommand)),

                    new Image().Row(Row.Photo)
                        .Bind(Image.SourceProperty, nameof(AddPhotoViewModel.PhotoImageSource)),

                    new Frame
                    {
                        Content = new StackLayout
                        {
                            Children =
                            {
                                new Label { Text = "Validating Photo" }.TextCenter()
                                    .Bind(IsVisibleProperty, nameof(AddPhotoViewModel.IsValidatingPhoto)),

                                new ActivityIndicator()
                                    .Bind(IsVisibleProperty, nameof(AddPhotoViewModel.IsValidatingPhoto))
                                    .Bind(ActivityIndicator.IsRunningProperty, nameof(AddPhotoViewModel.IsValidatingPhoto))
                            }
                        }
                    }.Center()
                     .Bind(IsVisibleProperty, nameof(AddPhotoViewModel.IsValidatingPhoto))
                }
            };
        }

        enum Row { Title, TakePhotoButton, Photo }

        void HandleInvalidPhotoSubmitted(object sender, InvalidPhotoEventArgs e)
        {
            var errorString = new StringBuilder();

            if (e.InvalidAPIKey)
                errorString.AppendLine("Invalid API Key");
            if (e.InternetConnectionFailed)
                errorString.AppendLine("Internet Connection Failed");
            if (!e.DoesImageContainAcceptablePhotoTags)
                errorString.AppendLine("No Plant Detected");
            if (e.DoesContainAdultContent)
                errorString.AppendLine("Adult Content Detected");
            if (e.DoesContainRacyContent)
                errorString.AppendLine("Racy Content Detected");

            errorString.Remove(errorString.Length - 1, 1);

            MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Error", errorString.ToString(), "Ok"));
        }

        void HandleDuplicateImageNameDetected(object sender, EventArgs e) =>
            MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Error: Duplicate Photo Name", $"A Photo Titled {_photoImageNameEntry.Text} Already Exists", "Ok"));

        void HandleDisplayNoCameraAvailableAlert(object sender, EventArgs e) =>
            MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Error", "Camera Unavailable", "Ok"));

        async void HandlePhotoSavedToDatabaseCompleted(object sender, EventArgs e) => await DismissPage();

        async void HandleCancelButtonClicked(object sender, EventArgs e) => await DismissPage();

        Task DismissPage() => MainThread.InvokeOnMainThreadAsync(() => Navigation.PopModalAsync());
    }
}
