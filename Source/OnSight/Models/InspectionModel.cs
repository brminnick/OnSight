using System;

using SQLite;

namespace OnSight
{
	public class InspectionModel
	{
        #region Constructors
        public InspectionModel() => Id = Guid.NewGuid().ToString();
#endregion

        #region Properties
        [Unique, PrimaryKey]
		public string Id { get; set; }

		public string InspectionTitle { get; set; }

		public DateTimeOffset InspectionDateUTC { get; set; }

		public string InspectionNotes { get; set; }
		#endregion
	}
}
