using System.Collections.Generic;

namespace OnSight
{
	public class InspectionListViewModel : BaseViewModel
	{
		#region Fields
		List<InspectionModel> _visibleInspectionModelList;
		#endregion

		#region Constructors
		public InspectionListViewModel()
		{
			
		}
		#endregion

		#region Properties
		List<InspectionModel> VisibleInspectionModelList
		{
			get { return _visibleInspectionModelList; }
			set { SetProperty(ref _visibleInspectionModelList, value); }
		}
		#endregion
	}
}
