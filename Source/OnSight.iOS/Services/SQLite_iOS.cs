using System;
using System.IO;

using SQLite;

using Xamarin.Forms;

using OnSight.iOS;

[assembly: Dependency(typeof(SQLite_iOS))]
namespace OnSight.iOS
{
	public class SQLite_iOS : ISQLite
	{
		#region ISQLite implementation
		public SQLiteAsyncConnection GetConnection()
		{
			var sqliteFilename = "InspectionModelDatabase.db3";
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string libraryPath = Path.Combine(folderPath, "..", "Library");
			var sqlLiteDatabasePath = Path.Combine(libraryPath, sqliteFilename);

			var sqlLiteDatabaseConnection = new SQLiteAsyncConnection(sqlLiteDatabasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

			return sqlLiteDatabaseConnection;
		}
		#endregion
	}
}

