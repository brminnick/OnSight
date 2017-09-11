using System.Windows.Input;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace OnSight
{
	public class InspectionDetailsViewModel : BaseViewModel
	{
		#region Constant Fields
		readonly string _inspectionId;
		#endregion

		#region Fields
		string _titleText, _notesText = "Notes";
		ICommand _saveDataCommand;
		InspectionModel _inspectionModel;
		#endregion

		#region Constructors
		public InspectionDetailsViewModel(string inspectionId)
		{
			_inspectionId = inspectionId;
			Task.Run(async () => await UpdateInspectionModel());
		}
		#endregion

		#region Properties 
		public ICommand SaveDataCommand => _saveDataCommand ??
			(_saveDataCommand = new Command(async () => await ExecuteSaveDataCommand()));

		public string TitleText
		{
			get => _titleText;
			set => SetProperty(ref _titleText, value);
		}

		public string NotesText
		{
			get => _notesText;
			set => SetProperty(ref _notesText, value);
		}

		InspectionModel InspectionModel
		{
			get => _inspectionModel;
			set => SetProperty(ref _inspectionModel, value, async () => await UpdateInspectionModel());
		}
		#endregion

		#region Methods
		async Task ExecuteSaveDataCommand()
		{
			InspectionModel.InspectionNotes = NotesText;
			InspectionModel.InspectionTitle = TitleText;

			await InspectionModelDatabase.SaveInspectionModelAsync(InspectionModel);
		}

		async Task UpdateInspectionModel()
		{
            if (InspectionModel?.Id.Equals(_inspectionId) ?? false)
				return;

			InspectionModel = await InspectionModelDatabase.GetInspectionModelAsync(_inspectionId);
			NotesText = InspectionModel.InspectionNotes;
			TitleText = InspectionModel.InspectionTitle;
		}
		#endregion
	}
}
