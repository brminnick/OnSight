using SQLite;

namespace OnSight
{
	public class NoteModel
	{
		[Unique]
		public string Title { get; set; }

		public string Details { get; set; }
	}
}
