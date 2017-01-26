using Xamarin.Forms;
using InvestmentDataSampleApp;
namespace OnSight
{
	public class InspectionListPage : BaseContentPage<InspectionListViewModel>
	{
		#region Constructors
		public InspectionListPage()
		{
			var relativeLayout = new RelativeLayout();

			var listView = new ListView(ListViewCachingStrategy.RecycleElement)
			{
				ItemTemplate = new DataTemplate(typeof(HSBImageCell)),
				IsPullToRefreshEnabled = true,
				SeparatorVisibility = SeparatorVisibility.None
			};
			listView.SetBinding(ListView.RefreshCommandProperty, nameof(ViewModel.PullToRefreshCommand));
			listView.SetBinding(ListView.ItemsSourceProperty, nameof(ViewModel.VisibleInspectionModelList));

			relativeLayout.Children.Add(listView,
									   Constraint.Constant(0),
									   Constraint.Constant(0),
									   Constraint.RelativeToParent(parent => parent.Width),
									   Constraint.RelativeToParent(parent => parent.Height));

			var addInspectionToolbarItem = new ToolbarItem();
			addInspectionToolbarItem.Icon = "Add";
			addInspectionToolbarItem.Clicked += (sender, e) =>
			{
				var addInspectionView = new AddInspectionView();

				relativeLayout.Children.Add(addInspectionView,
										   Constraint.Constant(0),
										   Constraint.Constant(0));

				addInspectionView.DisplayView();
			};
			ToolbarItems.Add(addInspectionToolbarItem);

			Title = "On Sight";

			NavigationPage.SetBackButtonTitle(this, "Home");

			Content = relativeLayout;

			ViewModel.PullToRefreshCompleted += (sender, e) => Device.BeginInvokeOnMainThread(listView.EndRefresh);
		}
		#endregion
	}
}
