using System;

using Xamarin.Forms;
using System.Threading.Tasks;

namespace OnSight
{
	public class InspectionListPage : ContentPage
	{
		#region Constant Fields
		readonly ListView _listView;
		readonly InspectionListViewModel _viewModel;
		#endregion

		#region Constructors
		public InspectionListPage()
		{
			_viewModel = new InspectionListViewModel();
			BindingContext = _viewModel;

			var relativeLayout = new RelativeLayout();

			_listView = new ListView(ListViewCachingStrategy.RecycleElement)
			{
				ItemTemplate = new DataTemplate(typeof(HSBViewCell)),
				IsPullToRefreshEnabled = true,
				SeparatorVisibility = SeparatorVisibility.None,
				RowHeight = 50
			};
			_listView.SetBinding(ListView.RefreshCommandProperty, nameof(_viewModel.PullToRefreshCommand));
			_listView.SetBinding(ListView.ItemsSourceProperty, nameof(_viewModel.VisibleInspectionModelList));

			relativeLayout.Children.Add(_listView,
									   Constraint.Constant(0),
									   Constraint.Constant(0),
									   Constraint.RelativeToParent(parent => parent.Width),
									   Constraint.RelativeToParent(parent => parent.Height));

			var addInspectionToolbarItem = new ToolbarItem();
			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
				case Device.Android:
					addInspectionToolbarItem.Icon = "Add";
					break;
				case Device.Windows:
					addInspectionToolbarItem.Icon = "Assets/Add.png";
					break;
				default:
					throw new Exception("Runtime Platform Not Supported");
			}
			addInspectionToolbarItem.Clicked += (sender, e) =>
			{
				var addInspectionView = new AddInspectionView();

				relativeLayout.Children.Add(addInspectionView,
										   Constraint.Constant(0),
										   Constraint.Constant(0));

				addInspectionView.DisplayView();
			};
			ToolbarItems.Add(addInspectionToolbarItem);

			Title = "OnSight";

			NavigationPage.SetBackButtonTitle(this, "Home");

			Content = relativeLayout;

		}
		#endregion

		#region Methods
		protected override void OnAppearing()
		{
			base.OnAppearing();

			_viewModel.PullToRefreshCompleted += HandlePullToRefreshCompleted;
			_listView.ItemTapped += HandleItemTapped;

			UpdateListData();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.PullToRefreshCompleted -= HandlePullToRefreshCompleted;
			_listView.ItemTapped -= HandleItemTapped;
		}

		void UpdateListData()
		{
			Device.BeginInvokeOnMainThread(_listView.BeginRefresh);
		}

		void HandleItemTapped(object sender, ItemTappedEventArgs e)
		{
			var selectedInspectionModel = e.Item as InspectionModel;

			Device.BeginInvokeOnMainThread(async () =>
			{
				await Navigation.PushAsync(new InspectionDetailsPage(selectedInspectionModel.Id));
				_listView.SelectedItem = null;
			});
		}

		void HandlePullToRefreshCompleted(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(_listView.EndRefresh);
		}
		#endregion
	}
}
