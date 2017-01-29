using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace OnSight
{
	public class InspectionDetailsViewModel : BaseViewModel
	{
		#region Constant Fields
		readonly int _inspectionId;
		#endregion

		#region Fields
		string _titleText;
		Command _saveDataCommand;
		InspectionModel _inspectionModel;
		#endregion

		#region Constructors
		public InspectionDetailsViewModel(int inspectionId)
		{
			_inspectionId = inspectionId;
			Task.Run(async () => await UpdateInspectionModel());
		}
		#endregion

		#region Events

		#endregion

		#region Properties 
		public Command SaveDataCommand => _saveDataCommand ??
			(_saveDataCommand = new Command(async () => await ExecuteSaveDataCommand()));

		public string TitleText
		{
			get { return _titleText; }
			set { SetProperty(ref _titleText, value); }
		}

		InspectionModel InspectionModel
		{
			get { return _inspectionModel; }
			set { SetProperty(ref _inspectionModel, value, HandleInspectionModelUpdated); }
		}

		#endregion

		#region Methods
		async Task ExecuteSaveDataCommand()
		{
			InspectionModel.InspectionTitle = TitleText;

			await InspectionModelDatabase.SaveInspectionModelAsync(InspectionModel);
		}

		async Task UpdateInspectionModel()
		{
			if (InspectionModel?.Id == _inspectionId)
				return;

			InspectionModel = await InspectionModelDatabase.GetInspectionModelAsync(_inspectionId);
		}

		void HandleInspectionModelUpdated()
		{
			TitleText = InspectionModel.InspectionTitle;
		}
		#endregion
	}
}
