using System;
using System.Collections;
using System.Linq;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OnSight
{
    class InspectionListPage : BaseContentPage<InspectionListViewModel>
    {
        public InspectionListPage() : base(new InspectionListViewModel())
        {
            Title = "OnSight";
            NavigationPage.SetBackButtonTitle(this, "Home");

            ToolbarItems.Add(new ToolbarItem
            {
                IconImageSource = Device.RuntimePlatform switch
                {
                    Device.iOS => "Add",
                    Device.Android => "Add",
                    Device.UWP => "Assets/Add.png",
                    _ => throw new NotSupportedException()
                }
            }.Invoke(addInspectionToolbarItem => addInspectionToolbarItem.Clicked += HandleAddInspectionToolbarItemClicked));

            Content = new Grid
            {
                Children =
                {
                    new RefreshView
                    {
                        Content = new CollectionView
                        {
                            SelectionMode = SelectionMode.Single,
                            ItemTemplate = new InspectionDataTemplate()
                        }.Bind(CollectionView.ItemsSourceProperty, nameof(InspectionListViewModel.VisibleInspectionModelList))
                         .Invoke(collectionView => collectionView.SelectionChanged += HandleSelectionChanged)

                     }.Bind(RefreshView.IsRefreshingProperty, nameof(InspectionListViewModel.IsListRefreshing))
                      .Bind(RefreshView.CommandProperty, nameof(InspectionListViewModel.PullToRefreshCommand))
                }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Content is Layout<View> layout
                && layout.Children.OfType<RefreshView>().First() is RefreshView refreshView
                && refreshView.Content is CollectionView collectionView
                && IsNullOrEmpty(collectionView.ItemsSource))
            {
                refreshView.IsRefreshing = true;
            }

            static bool IsNullOrEmpty(in IEnumerable? enumerable) => !enumerable?.GetEnumerator().MoveNext() ?? true;
        }

        void HandleAddInspectionToolbarItemClicked(object sender, EventArgs e)
        {
            var addInspectionView = new AddInspectionView();

            var layout = (Layout<View>)Content;
            layout.Children.Add(addInspectionView.FillExpand());

            addInspectionView.DisplayView();
        }

        void HandleSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var collectionView = (CollectionView)sender;
            collectionView.SelectedItem = null;

            if (e.CurrentSelection.FirstOrDefault() is InspectionModel selectedInspectionModel)
                MainThread.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new InspectionDetailsPage(selectedInspectionModel.Id)));
        }
    }
}
