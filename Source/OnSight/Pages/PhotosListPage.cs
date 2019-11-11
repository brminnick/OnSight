using System;
using System.Collections;
using Xamarin.Forms;

namespace OnSight
{
    public class PhotosListPage : ContentPage
    {
        readonly string _inspectionId;

        public PhotosListPage(string inspectionId)
        {
            _inspectionId = inspectionId;
            BindingContext = new PhotosListViewModel(inspectionId);

            var photosListView = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(PhotoListImageCell)),
                IsPullToRefreshEnabled = true
            };
            photosListView.ItemSelected += HandleItemSelected;
            photosListView.SetBinding(ListView.RefreshCommandProperty, nameof(PhotosListViewModel.RefreshCommand));
            photosListView.SetBinding(ListView.IsRefreshingProperty, nameof(PhotosListViewModel.IsListRefreshing));
            photosListView.SetBinding(ListView.ItemsSourceProperty, nameof(PhotosListViewModel.VisiblePhotoModelList));

            var addPhotoToolbarItem = new ToolbarItem
            {
                IconImageSource = Device.RuntimePlatform switch
                {
                    Device.iOS => "Add",
                    Device.Android => "Add",
                    Device.UWP => "Assets/Add.png",
                    _ => throw new NotSupportedException()
                }
            };
            addPhotoToolbarItem.Clicked += HandleAddPhotoToolbarItemClicked;
            ToolbarItems.Add(addPhotoToolbarItem);

            Title = "Photos";

            Content = photosListView;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Content is ListView listView
                && IsNullOrEmpty(listView.ItemsSource))
            {
                listView.BeginRefresh();
            }

            static bool IsNullOrEmpty(in IEnumerable? enumerable) => !enumerable?.GetEnumerator().MoveNext() ?? true;
        }

        void HandleItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var listView = (ListView)sender;
            listView.SelectedItem = null;
        }

        void HandleAddPhotoToolbarItemClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var addPhotoNavigationPage = new NavigationPage(new AddPhotoPage(_inspectionId))
                {
                    BarBackgroundColor = ColorConstants.NavigationBarBackgroundColor,
                    BarTextColor = ColorConstants.NavigationBarTextColor
                };

                await Navigation.PushModalAsync(addPhotoNavigationPage);
            });
        }
    }
}
