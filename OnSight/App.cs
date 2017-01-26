using Xamarin.Forms;

namespace OnSight
{
	public class App : Application
	{
		public App()
		{
			MainPage = new NavigationPage(new InspectionListPage())
			{
				BarBackgroundColor = Color.FromHex("00538A"),
				BarTextColor = Color.White
			};
		}
	}
}
