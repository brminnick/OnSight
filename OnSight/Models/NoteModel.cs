using SQLite;

namespace OnSight
{
	public class NoteModel
	{
		#region Properties
		[Unique, AutoIncrement, PrimaryKey]
		public int Id { get; set;}

		public int InspectionModelId { get; set; }

		public string Title { get; set; }

		public string Details { get; set; }
		#endregion
	}
}
