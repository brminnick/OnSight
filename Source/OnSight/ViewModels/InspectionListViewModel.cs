using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

		#region Constructors
		public InspectionListViewModel()
		{
			Task.Run(async () => await RefreshData());
		}
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
			get { return _visibleInspectionModelList; }
			set { SetProperty(ref _visibleInspectionModelList, value); }
		}

		public string TitleEntryText
		{
			get { return _titleEntryText; }
			set { SetProperty(ref _titleEntryText, value); }
		}
		#endregion

		#region Methods
		async Task ExecutePullToRefreshCommand()
		{
			await DisplayRefreshingIndicator(500);
			await RefreshData();
			OnPullToRefreshCompleted();
		}

		async Task ExecuteSubmitButtonCommand()
		{
			var inspectionModel = new InspectionModel
			{
				InspectionTitle = TitleEntryText,
				InspectionDateUTC = DateTime.UtcNow
			};

			var inspectionId = await InspectionModelDatabase.SaveInspectionModelAsync(inspectionModel);

			await RefreshData();
		}

		async Task RefreshData()
		{
			VisibleInspectionModelList = await InspectionModelDatabase.GetAllInspectionModelsAsync();
		}

		async Task DisplayRefreshingIndicator(int indicatorDisplayTimeInMilliseconds)
		{
			await Task.Delay(TimeSpan.FromMilliseconds(indicatorDisplayTimeInMilliseconds));
		}

		void OnPullToRefreshCompleted()
		{
			PullToRefreshCompleted?.Invoke(null, EventArgs.Empty);
		}
		#endregion
	}
}
