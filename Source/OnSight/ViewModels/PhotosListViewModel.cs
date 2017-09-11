using System;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xamarin.Forms;

namespace OnSight
{
    public class PhotosListViewModel : BaseViewModel
    {
        #region Constant Fields
        readonly string _inspectionId;
        #endregion

        #region Fields
		ICommand _refreshCommand;
        List<PhotoModel> _visibleNoteModelList;
        #endregion

        #region Constructors
        public PhotosListViewModel(string inspectionId) => _inspectionId = inspectionId;

        #endregion

        #region Properties
        public ICommand RefreshCommand => _refreshCommand ??
            (_refreshCommand = new Command(async () => await ExecuteRefreshCommand()));

        public List<PhotoModel> VisiblePhotoModelList
        {
            get => _visibleNoteModelList;
            set => SetProperty(ref _visibleNoteModelList, value);
        }
        #endregion

        #region Events
        public event EventHandler PullToRefreshCompleted;
        #endregion

        #region Methods
        async Task ExecuteRefreshCommand()
        {
            await DisplayRefreshingIndicator(500);
            await RefreshData();
            OnPullToRefreshCompleted();
        }

		async Task RefreshData() =>
		    VisiblePhotoModelList = await PhotoModelDatabase.GetAllPhotosForInspection(_inspectionId);

        async Task DisplayRefreshingIndicator(int indicatorDisplayTimeInSeconds) =>
            await Task.Delay(TimeSpan.FromMilliseconds(indicatorDisplayTimeInSeconds));

        void OnPullToRefreshCompleted() =>
            PullToRefreshCompleted?.Invoke(this, EventArgs.Empty);
        #endregion
    }

}
