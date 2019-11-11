using Xamarin.Forms;

namespace OnSight
{
    public class App : Application
    {
        public App()
        {
            MainPage = new NavigationPage(new InspectionListPage())
            {
                BarBackgroundColor = ColorConstants.NavigationBarBackgroundColor,
                BarTextColor = ColorConstants.NavigationBarTextColor
            };
        }
    }
}
