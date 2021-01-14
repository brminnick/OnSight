using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using SQLite;

namespace OnSight
{
    abstract class BaseDatabase
    {
        static readonly string _databasePath = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, $"{nameof(OnSight)}.db3");

        static readonly Lazy<SQLiteAsyncConnection> _databaseConnectionHolder = new(() => new SQLiteAsyncConnection(_databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache));

        static SQLiteAsyncConnection DatabaseConnection => _databaseConnectionHolder.Value;

        protected static async Task<SQLiteAsyncConnection> GetDatabaseConnection<T>()
        {
            if (!DatabaseConnection.TableMappings.Any(x => x.MappedType.Name == typeof(T).Name))
            {
                await DatabaseConnection.EnableWriteAheadLoggingAsync().ConfigureAwait(false);
                await DatabaseConnection.CreateTablesAsync(CreateFlags.None, typeof(T)).ConfigureAwait(false);
            }

            return DatabaseConnection;
        }
    }
}
