using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OnSight
{
	public class PhotosListViewModel : BaseViewModel
	{
		#region Constant Fields
		readonly int _inspectionId;
		#endregion

		#region Fields
		List<PhotoModel> _visibleNoteModelList;
		Command _refreshCommand;
		#endregion

		#region Constructors
		public PhotosListViewModel(int inspectionId)
		{
			_inspectionId = inspectionId;
		}

		#endregion

		#region Properties
		public Command RefreshCommand => _refreshCommand ??
		(_refreshCommand = new Command(async () => await ExecuteRefreshCommand()));

		public List<PhotoModel> VisiblePhotoModelList
		{
			get { return _visibleNoteModelList; }
			set { SetProperty(ref _visibleNoteModelList, value); }
		}
		#endregion

		#region Events
		public event EventHandler PullToRefreshCompleted;
		#endregion

		#region Methods
		async Task RefreshData()
		{
			VisiblePhotoModelList = await InspectionModelDatabase.GetAllPhotosForInspection(_inspectionId);
		}

		async Task ExecuteRefreshCommand()
		{
			await DisplayRefreshingIndicator(500);
			await RefreshData();
			OnPullToRefreshCompleted();
		}

		async Task DisplayRefreshingIndicator(int indicatorDisplayTimeInSeconds)
		{
			await Task.Delay(TimeSpan.FromMilliseconds(indicatorDisplayTimeInSeconds));
		}

		void OnPullToRefreshCompleted()
		{
			PullToRefreshCompleted?.Invoke(null, EventArgs.Empty);
		}
		#endregion
	}

}
