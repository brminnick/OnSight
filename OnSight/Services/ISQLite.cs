using SQLite;

namespace OnSight
{
	public interface ISQLite
	{
		SQLiteConnection GetConnection();
	}
}

