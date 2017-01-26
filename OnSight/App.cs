using Xamarin.Forms;

namespace OnSight
{
	public class App : Application
	{
		#region Fields
		static InspectionModelDatabase _database;
		#endregion

		#region Constructors
		public App()
		{
			MainPage = new NavigationPage(new InspectionListPage())
			{
				BarBackgroundColor = Color.FromHex("00538A"),
				BarTextColor = Color.White
			};
		}
		#endregion

		#region Properties
		public static InspectionModelDatabase Database => _database ??
			(_database = new InspectionModelDatabase());
		#endregion
	}
}
