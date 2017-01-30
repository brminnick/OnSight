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

			Title = "OnSight";

			NavigationPage.SetBackButtonTitle(this, "Home");
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
