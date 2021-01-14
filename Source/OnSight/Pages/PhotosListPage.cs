using System;
using System.Collections;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OnSight
{
    class PhotosListPage : BaseContentPage<PhotosListViewModel>
    {
        readonly string _inspectionId;

        public PhotosListPage(string inspectionId) : base(new PhotosListViewModel(inspectionId))
        {
            _inspectionId = inspectionId;

            ToolbarItems.Add(new ToolbarItem
            {
                IconImageSource = Device.RuntimePlatform switch
                {
                    Device.iOS => "Add",
                    Device.Android => "Add",
                    Device.UWP => "Assets/Add.png",
                    _ => throw new NotSupportedException()
                }
            }.Invoke(addButton => addButton.Clicked += HandleAddPhotoToolbarItemClicked));

            Title = "Photos";

            Content = new RefreshView
            {
                Content = new CollectionView
                {
                    ItemTemplate = new PhotoDataTemplate()
                }.Bind(CollectionView.ItemsSourceProperty, nameof(PhotosListViewModel.VisiblePhotoModelList))

            }.Bind(RefreshView.CommandProperty, nameof(PhotosListViewModel.RefreshCommand))
             .Bind(RefreshView.IsRefreshingProperty, nameof(PhotosListViewModel.IsListRefreshing));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Content is RefreshView refreshView
                && refreshView.Content is CollectionView collectionView
                && IsNullOrEmpty(collectionView.ItemsSource))
            {
                refreshView.IsRefreshing = true;
            }

            static bool IsNullOrEmpty(in IEnumerable? enumerable) => !enumerable?.GetEnumerator().MoveNext() ?? true;
        }

        void HandleAddPhotoToolbarItemClicked(object sender, EventArgs e) => MainThread.BeginInvokeOnMainThread(async () =>
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
