using System;

using Xamarin.Forms;

namespace OnSight
{
	public class PhotosListPage : ContentPage
	{
		#region Constant Fields
		readonly int _inspectionId;
		readonly ListView _notesListView;
		readonly PhotosListViewModel _viewModel;
		readonly ToolbarItem _addPhotoToolbarItem;
		#endregion

		#region Constructors
		public PhotosListPage(int inspectionId)
		{
			_inspectionId = inspectionId;

			_viewModel = new PhotosListViewModel(inspectionId);
			BindingContext = _viewModel;

			_notesListView = new ListView
			{
				ItemTemplate = new DataTemplate(typeof(PhotosListTextCell)),
				IsPullToRefreshEnabled = true
			};
			_notesListView.SetBinding(ListView.RefreshCommandProperty, nameof(_viewModel.RefreshCommand));
			_notesListView.SetBinding(ListView.ItemsSourceProperty, nameof(_viewModel.VisiblePhotoModelList));

			_addPhotoToolbarItem = new ToolbarItem();
			_addPhotoToolbarItem.Icon = "Add";
			ToolbarItems.Add(_addPhotoToolbarItem);

			Title = "Photos";

			Content = _notesListView;
		}
		#endregion

		#region Methods
		protected override void OnAppearing()
		{
			base.OnAppearing();

			_addPhotoToolbarItem.Clicked += HandleAddPhotoToolbarItemClicked;
			_viewModel.PullToRefreshCompleted += HandlePullToRefreshCompleted;

			_notesListView.BeginRefresh();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_addPhotoToolbarItem.Clicked -= HandleAddPhotoToolbarItemClicked;
			_viewModel.PullToRefreshCompleted -= HandlePullToRefreshCompleted;
		}

		void HandlePullToRefreshCompleted(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(_notesListView.EndRefresh);
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
				_notesListView.SelectedItem = null;
			});
		}
		#endregion
	}
}
