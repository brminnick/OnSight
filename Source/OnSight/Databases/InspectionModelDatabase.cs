using System.Threading.Tasks;
using System.Collections.Generic;

namespace OnSight
{
    abstract class InspectionModelDatabase : BaseDatabase
    {
        public static async Task<IReadOnlyList<InspectionModel>> GetAllInspectionModelsAsync()
        {
            var databaseConnection = await GetDatabaseConnection<InspectionModel>().ConfigureAwait(false);

            return await databaseConnection.Table<InspectionModel>().ToListAsync().ConfigureAwait(false);
        }

        public static async Task<InspectionModel> GetInspectionModelAsync(string id)
        {
            var databaseConnection = await GetDatabaseConnection<InspectionModel>().ConfigureAwait(false);

            return await databaseConnection.Table<InspectionModel>().FirstAsync(x => x.Id.Equals(id));
        }

        public static async Task<int> SaveInspectionModelAsync(InspectionModel inspectionModel)
        {
            var databaseConnection = await GetDatabaseConnection<InspectionModel>().ConfigureAwait(false);

            return await databaseConnection.InsertOrReplaceAsync(inspectionModel).ConfigureAwait(false);
        }
    }
}

