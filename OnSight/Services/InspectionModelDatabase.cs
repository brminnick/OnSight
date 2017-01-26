using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using SQLite;
using Xamarin.Forms;

namespace OnSight
{
	public class InspectionModelDatabase
	{
		readonly SQLiteAsyncConnection _databaseConnection;

		public InspectionModelDatabase()
		{
			_databaseConnection = DependencyService.Get<ISQLite>()?.GetConnection();
			_databaseConnection.CreateTableAsync<InspectionModel>();
		}

		public async Task<List<InspectionModel>> GetAllInspectionModelsAsync()
		{
			return await _databaseConnection.Table<InspectionModel>().ToListAsync();
		}

		public async Task<InspectionModel> GetInspectionModelAsync(int id)
		{
			return await _databaseConnection.Table<InspectionModel>().Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
		}

		public async Task<int> SaveInspectionModelAsync(InspectionModel inspectionModel)
		{
			if (await GetInspectionModelAsync(inspectionModel.Id) != null)
			{
				await _databaseConnection.UpdateAsync(inspectionModel);
				return inspectionModel.Id;
			}

			return await _databaseConnection.InsertAsync(inspectionModel);
		}
	}
}

