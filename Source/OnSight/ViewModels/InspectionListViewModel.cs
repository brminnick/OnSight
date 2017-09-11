using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xamarin.Forms;

namespace OnSight
{
    public class InspectionListViewModel : BaseViewModel
    {
        #region Fields
        string _titleEntryText;
        Command _pullToRefreshCommand, _submitButtonCommand;
        List<InspectionModel> _visibleInspectionModelList;
        #endregion

        #region Events
        public event EventHandler PullToRefreshCompleted;
        #endregion

        #region Properties
        public Command PullToRefreshCommand => _pullToRefreshCommand ??
            (_pullToRefreshCommand = new Command(async () => await ExecutePullToRefreshCommand()));

        public Command SubmitButtonCommand => _submitButtonCommand ??
            (_submitButtonCommand = new Command(async () => await ExecuteSubmitButtonCommand()));

        public List<InspectionModel> VisibleInspectionModelList
        {
            get => _visibleInspectionModelList;
            set => SetProperty(ref _visibleInspectionModelList, value);
        }

        public string TitleEntryText
        {
            get => _titleEntryText;
            set => SetProperty(ref _titleEntryText, value);
        }
        #endregion

        #region Methods
        async Task ExecutePullToRefreshCommand()
        {
            await Task.WhenAll(RefreshData(), DisplayRefreshingIndicator(500));
            OnPullToRefreshCompleted();
        }

        async Task ExecuteSubmitButtonCommand()
        {
            var inspectionModel = new InspectionModel
            {
                InspectionTitle = TitleEntryText,
                InspectionDateUTC = DateTime.UtcNow
            };

            await InspectionModelDatabase.SaveInspectionModelAsync(inspectionModel);
            await RefreshData();
        }

        async Task RefreshData() =>
            VisibleInspectionModelList = await InspectionModelDatabase.GetAllInspectionModelsAsync();

        async Task DisplayRefreshingIndicator(int indicatorDisplayTimeInMilliseconds) =>
            await Task.Delay(TimeSpan.FromMilliseconds(indicatorDisplayTimeInMilliseconds));

        void OnPullToRefreshCompleted() =>
            PullToRefreshCompleted?.Invoke(this, EventArgs.Empty);
        #endregion
    }
}
