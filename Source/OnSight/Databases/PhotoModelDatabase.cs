using System.Threading.Tasks;
using System.Collections.Generic;

namespace OnSight
{
    abstract class PhotoModelDatabase : BaseDatabase
    {
		public static async Task<int> SavePhoto(PhotoModel photoModel)
		{
			var databaseConnection = await GetDatabaseConnection<PhotoModel>().ConfigureAwait(false);

            return await databaseConnection.InsertOrReplaceAsync(photoModel).ConfigureAwait(false);
		}

		public static async Task<PhotoModel> GetPhoto(string id)
		{
			var databaseConnection = await GetDatabaseConnection<PhotoModel>().ConfigureAwait(false);

            return await databaseConnection.Table<PhotoModel>().FirstAsync(x => x.Id.Equals(id)).ConfigureAwait(false);
		}

		public static async Task<IReadOnlyList<PhotoModel>> GetAllPhotosForInspection(string inspectionModelId)
		{
			var databaseConnection = await GetDatabaseConnection<PhotoModel>().ConfigureAwait(false);

            return await databaseConnection.Table<PhotoModel>().Where(x => x.InspectionModelId.Equals(inspectionModelId)).ToListAsync().ConfigureAwait(false);
		}
    }
}
