using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;

namespace OnSight
{
    public class PhotosListViewModel : BaseViewModel
    {
        readonly string _inspectionId;

        bool _isListRefreshing;
        ICommand? _refreshCommand;
        IReadOnlyList<PhotoModel> _visibleNoteModelList = Array.Empty<PhotoModel>();

        public PhotosListViewModel(string inspectionId) => _inspectionId = inspectionId;

        public ICommand RefreshCommand => _refreshCommand ??= new AsyncCommand(ExecuteRefreshCommand);

        public bool IsListRefreshing
        {
            get => _isListRefreshing;
            set => SetProperty(ref _isListRefreshing, value);
        }

        public IReadOnlyList<PhotoModel> VisiblePhotoModelList
        {
            get => _visibleNoteModelList;
            set => SetProperty(ref _visibleNoteModelList, value);
        }

        async Task ExecuteRefreshCommand()
        {
            try
            {
                await Task.WhenAll(RefreshData(), DisplayRefreshingIndicator(TimeSpan.FromMilliseconds(500))).ConfigureAwait(false);
            }
            finally
            {
                IsListRefreshing = false;
            }
        }

        async Task RefreshData() =>
            VisiblePhotoModelList = await PhotoModelDatabase.GetAllPhotosForInspection(_inspectionId).ConfigureAwait(false);

        Task DisplayRefreshingIndicator(TimeSpan timeSpan) => Task.Delay(timeSpan);
    }

}
