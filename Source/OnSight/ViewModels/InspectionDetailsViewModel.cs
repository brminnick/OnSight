using System;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;

namespace OnSight
{
    public class InspectionDetailsViewModel : BaseViewModel
    {
        readonly string _inspectionId;

        string _titleText = string.Empty, _notesText = "Notes";
        ICommand? _saveDataCommand;
        InspectionModel? _inspectionModel;

        public InspectionDetailsViewModel(string inspectionId)
        {
            _inspectionId = inspectionId;
            UpdateInspectionModel().SafeFireAndForget();
        }

        public ICommand SaveDataCommand => _saveDataCommand ??= new AsyncCommand(ExecuteSaveDataCommand);

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
            get => _inspectionModel ?? throw new NullReferenceException();
            set => SetProperty(ref _inspectionModel, value, async () => await UpdateInspectionModel().ConfigureAwait(false));
        }

        Task ExecuteSaveDataCommand()
        {
            InspectionModel = InspectionModel with { InspectionNotes = NotesText, InspectionTitle = TitleText };

            return InspectionModelDatabase.SaveInspectionModelAsync(InspectionModel);
        }

        async ValueTask UpdateInspectionModel()
        {
            if (_inspectionModel is null || _inspectionModel.Id != _inspectionId)
            {
                InspectionModel = await InspectionModelDatabase.GetInspectionModelAsync(_inspectionId).ConfigureAwait(false);
                NotesText = InspectionModel.InspectionNotes;
                TitleText = InspectionModel.InspectionTitle;
            }
        }
    }
}
