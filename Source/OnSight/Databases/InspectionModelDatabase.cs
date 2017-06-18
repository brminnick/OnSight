using System.Threading.Tasks;
using System.Collections.Generic;

namespace OnSight
{
    public abstract class InspectionModelDatabase : BaseDatabase
    {
        #region Methods
        public static async Task<List<InspectionModel>> GetAllInspectionModelsAsync()
        {
            var databaseConnection = await GetDatabaseConnectionAsync();

            return await databaseConnection?.Table<InspectionModel>()?.ToListAsync();
        }

        public static async Task<InspectionModel> GetInspectionModelAsync(int id)
        {
            var databaseConnection = await GetDatabaseConnectionAsync();

            return await databaseConnection?.Table<InspectionModel>()?.Where(x => x.Id.Equals(id))?.FirstOrDefaultAsync() ?? null;
        }

        public static async Task<int> SaveInspectionModelAsync(InspectionModel inspectionModel)
        {
            var databaseConnection = await GetDatabaseConnectionAsync();

            return await databaseConnection?.InsertOrReplaceAsync(inspectionModel);
        }
        #endregion
    }
}

