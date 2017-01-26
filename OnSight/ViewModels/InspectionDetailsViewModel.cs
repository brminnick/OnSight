using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace OnSight
{
	public class InspectionDetailsViewModel : BaseViewModel
	{
		#region Fields
		string _titleText;
		InspectionModel _inspectionModel;
		#endregion

		#region Constructors
		public InspectionDetailsViewModel()
		{
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
		#endregion

		#region Methods
		async Task ExecuteSaveButtonCommand()
		{
			_inspectionModel.InspectionTitle = TitleText;

			await App.Database.SaveInspectionModelAsync(_inspectionModel);
		}
		#endregion
	}
}
