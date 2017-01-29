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
		async Task ExecuteSaveButtonCommand()
		{
			InspectionModel.InspectionTitle = TitleText;

			await App.Database.SaveInspectionModelAsync(InspectionModel);
		}

		async Task UpdateInspectionModel()
		{
			if (InspectionModel?.Id == _inspectionId)
				return;

			InspectionModel = await App.Database.GetInspectionModelAsync(_inspectionId);
		}

		void HandleInspectionModelUpdated()
		{
			TitleText = InspectionModel.InspectionTitle;
		}
		#endregion
	}
}
