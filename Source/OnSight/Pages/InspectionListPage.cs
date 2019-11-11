using System;
using System.Collections;
using System.Linq;
using Xamarin.Forms;

namespace OnSight
{
    public class InspectionListPage : ContentPage
    {
        public InspectionListPage()
        {
            BindingContext = new InspectionListViewModel();

            var relativeLayout = new RelativeLayout();

            var listView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemTemplate = new DataTemplate(typeof(InspectionListViewCell)),
                IsPullToRefreshEnabled = true,
                SeparatorVisibility = SeparatorVisibility.None,
                RowHeight = 50
            };
            listView.SetBinding(ListView.IsRefreshingProperty, nameof(InspectionListViewModel.IsListViewRefreshing));
            listView.SetBinding(ListView.RefreshCommandProperty, nameof(InspectionListViewModel.PullToRefreshCommand));
            listView.SetBinding(ListView.ItemsSourceProperty, nameof(InspectionListViewModel.VisibleInspectionModelList));
            listView.ItemTapped += HandleItemTapped;

            relativeLayout.Children.Add(listView,
                                       Constraint.Constant(0),
                                       Constraint.Constant(0),
                                       Constraint.RelativeToParent(parent => parent.Width),
                                       Constraint.RelativeToParent(parent => parent.Height));

            var addInspectionToolbarItem = new ToolbarItem
            {
                IconImageSource = Device.RuntimePlatform switch
                {
                    Device.iOS => "Add",
                    Device.Android => "Add",
                    Device.UWP => "Assets/Add.png",
                    _ => throw new NotSupportedException()
                }
            };
            addInspectionToolbarItem.Clicked += HandleAddInspectionToolbarItemClicked;
            ToolbarItems.Add(addInspectionToolbarItem);

            Title = "OnSight";

            NavigationPage.SetBackButtonTitle(this, "Home");

            Content = relativeLayout;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Content is RelativeLayout relativeLayout
                && relativeLayout.Children.OfType<ListView>().First() is ListView listView
                && IsNullOrEmpty(listView.ItemsSource))
            {
                listView.BeginRefresh();
            }

            static bool IsNullOrEmpty(in IEnumerable? enumerable) => !enumerable?.GetEnumerator().MoveNext() ?? true;
        }

        void HandleAddInspectionToolbarItemClicked(object sender, EventArgs e)
        {
            var addInspectionView = new AddInspectionView();

            var relativeLayout = (RelativeLayout)Content;

            relativeLayout.Children.Add(addInspectionView,
                                       Constraint.Constant(0),
                                       Constraint.Constant(0));

            addInspectionView.DisplayView();
        }

        void HandleItemTapped(object sender, ItemTappedEventArgs e)
        {
            var listView = (ListView)sender;
            listView.SelectedItem = null;

            if (e?.Item is InspectionModel selectedInspectionModel)
            {
                Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new InspectionDetailsPage(selectedInspectionModel.Id)));
            }
        }
    }
}
