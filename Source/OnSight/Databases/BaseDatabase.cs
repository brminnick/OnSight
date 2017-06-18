using System;
using System.Threading.Tasks;

using SQLite;

using Xamarin.Forms;

namespace OnSight
{
    public abstract class BaseDatabase
    {
        #region Constant Fields
        static readonly Lazy<SQLiteAsyncConnection> _databaseConnectionHolder =
            new Lazy<SQLiteAsyncConnection>(() => DependencyService.Get<ISQLite>().GetConnection());
        #endregion

        #region Fields
        static bool _isDatabaseInitialized;
        #endregion

        #region Properties
        static SQLiteAsyncConnection DatabaseConnection => _databaseConnectionHolder.Value;
        #endregion

        #region Methods
        protected static async Task<SQLiteAsyncConnection> GetDatabaseConnectionAsync()
        {
            if (!_isDatabaseInitialized)
                await InitializeDatabase();

            return DatabaseConnection;
        }

        static async Task InitializeDatabase()
        {
            await DatabaseConnection.CreateTablesAsync<InspectionModel, PhotoModel>();
            _isDatabaseInitialized = true;
        }
        #endregion
    }
}
