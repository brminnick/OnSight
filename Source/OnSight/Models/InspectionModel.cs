using System;

using SQLite;

namespace OnSight
{
	public class InspectionModel
	{
		#region Properties
		[Unique, AutoIncrement, PrimaryKey]
		public int Id { get; set; }

		public string InspectionTitle { get; set; }

		public DateTime InspectionDateUTC { get; set; }

		public string InspectionNotes { get; set; }
		#endregion
	}
}
