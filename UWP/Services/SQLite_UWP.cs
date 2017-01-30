using System.IO;

using Windows.Storage;

using SQLite;

using Xamarin.Forms;

using OnSight;
using OnSight.UWP;

[assembly: Dependency(typeof(SQLite_UWP))]
namespace OnSight.UWP
{
    public class SQLite_UWP : ISQLite
    {
        #region ISQLite implementation
        public SQLiteAsyncConnection GetConnection()
        {
            var sqliteFilename = "InspectionModelDatabase.db3";
            string sqlLiteDataPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, sqliteFilename);
            var sqlLiteConnection = new SQLiteAsyncConnection(sqlLiteDataPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            return sqlLiteConnection;
        }
    }
    #endregion
}