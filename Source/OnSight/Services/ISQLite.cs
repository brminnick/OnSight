using SQLite;

namespace OnSight
{
	public interface ISQLite
	{
		SQLiteAsyncConnection GetConnection();
	}
}

