using System.IO;

using SQLite;

using Xamarin.Forms;

using OnSight.Droid;

[assembly: Dependency(typeof(SQLite_Android))]
namespace OnSight.Droid
{
	public class SQLite_Android : ISQLite
	{
		#region ISQLite implementation
		public SQLiteConnection GetConnection()
		{
			var sqliteFilename = "InspectionModelDatabase.db3";
			string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			var sqlLiteDatabasePath = Path.Combine(documentsPath, sqliteFilename);

			var sqlLiteConnection = new SQLiteConnection(sqlLiteDatabasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

			return sqlLiteConnection;
		}
		#endregion
	}
}

