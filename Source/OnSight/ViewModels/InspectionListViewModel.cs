﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;

namespace OnSight
{
    public class InspectionListViewModel : BaseViewModel
    {
        bool _isListRefreshing;
        string _titleEntryText = string.Empty;
        ICommand? _pullToRefreshCommand, _submitButtonCommand;
        IReadOnlyList<InspectionModel> _visibleInspectionModelList = Array.Empty<InspectionModel>();

        public ICommand PullToRefreshCommand => _pullToRefreshCommand ??= new AsyncCommand(ExecutePullToRefreshCommand);
        public ICommand SubmitButtonCommand => _submitButtonCommand ??= new AsyncCommand(ExecuteSubmitButtonCommand);

        public bool IsListRefreshing
        {
            get => _isListRefreshing;
            set => SetProperty(ref _isListRefreshing, value);
        }

        public IReadOnlyList<InspectionModel> VisibleInspectionModelList
        {
            get => _visibleInspectionModelList;
            set => SetProperty(ref _visibleInspectionModelList, value);
        }

        public string TitleEntryText
        {
            get => _titleEntryText;
            set => SetProperty(ref _titleEntryText, value);
        }

        async Task ExecutePullToRefreshCommand()
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

        async Task ExecuteSubmitButtonCommand()
        {
            var inspectionModel = new InspectionModel
            {
                InspectionTitle = TitleEntryText,
                InspectionDateUTC = DateTime.UtcNow
            };

            await InspectionModelDatabase.SaveInspectionModelAsync(inspectionModel).ConfigureAwait(false);
            await RefreshData().ConfigureAwait(false);
        }

        async Task RefreshData() =>
            VisibleInspectionModelList = await InspectionModelDatabase.GetAllInspectionModelsAsync().ConfigureAwait(false);

        Task DisplayRefreshingIndicator(int indicatorDisplayTimeInMilliseconds) =>
            Task.Delay(TimeSpan.FromMilliseconds(indicatorDisplayTimeInMilliseconds));
    }
}
