using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;

namespace OnSight
{
    public class PhotosListViewModel : BaseViewModel
    {
        readonly string _inspectionId;

        bool _isListRefreshing;
        ICommand? _refreshCommand;
        List<PhotoModel> _visibleNoteModelList = Enumerable.Empty<PhotoModel>().ToList();

        public PhotosListViewModel(string inspectionId) => _inspectionId = inspectionId;

        public ICommand RefreshCommand => _refreshCommand ??= new AsyncCommand(ExecuteRefreshCommand);

        public bool IsListRefreshing
        {
            get => _isListRefreshing;
            set => SetProperty(ref _isListRefreshing, value);
        }

        public List<PhotoModel> VisiblePhotoModelList
        {
            get => _visibleNoteModelList;
            set => SetProperty(ref _visibleNoteModelList, value);
        }

        async Task ExecuteRefreshCommand()
        {
            try
            {
                await Task.WhenAll(RefreshData(), DisplayRefreshingIndicator(500)).ConfigureAwait(false);
            }
            finally
            {
                IsListRefreshing = false;
            }
        }

        async Task RefreshData() =>
            VisiblePhotoModelList = await PhotoModelDatabase.GetAllPhotosForInspection(_inspectionId);

        Task DisplayRefreshingIndicator(int indicatorDisplayTimeInSeconds) =>
            Task.Delay(TimeSpan.FromMilliseconds(indicatorDisplayTimeInSeconds));
    }

}
