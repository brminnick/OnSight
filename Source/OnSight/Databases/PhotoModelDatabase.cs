using System.Threading.Tasks;
using System.Collections.Generic;

namespace OnSight
{
    public abstract class PhotoModelDatabase : BaseDatabase
    {
		public static async Task<int> SavePhoto(PhotoModel photoModel)
		{
			var databaseConnection = await GetDatabaseConnectionAsync();

			return await databaseConnection?.InsertOrReplaceAsync(photoModel);
		}

		public static async Task<PhotoModel> GetPhoto(string id)
		{
			var databaseConnection = await GetDatabaseConnectionAsync();

			return await databaseConnection?.Table<PhotoModel>()?.Where(x => x.Id.Equals(id))?.FirstOrDefaultAsync();
		}

		public static async Task<List<PhotoModel>> GetAllPhotosForInspection(string inspectionModelId)
		{
			var databaseConnection = await GetDatabaseConnectionAsync();

			return await databaseConnection?.Table<PhotoModel>().Where(x => x.InspectionModelId.Equals(inspectionModelId)).ToListAsync();
		}
    }
}
