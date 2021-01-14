using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace OnSight
{
    public class App : Xamarin.Forms.Application
    {
        public App()
        {
            var navigationPage = new Xamarin.Forms.NavigationPage(new InspectionListPage())
            {
                BarBackgroundColor = ColorConstants.NavigationBarBackgroundColor,
                BarTextColor = ColorConstants.NavigationBarTextColor
            };
            navigationPage.On<iOS>().SetPrefersLargeTitles(true);

            MainPage = navigationPage;
        }
    }

    static class MarkupExtensions
    {
        public static GridLength StarGridLength(double value) => new(value, GridUnitType.Star);
        public static GridLength StarGridLength(int value) => StarGridLength((double)value);

        public static GridLength AbsoluteGridLength(double value) => new(value, GridUnitType.Absolute);
        public static GridLength AbsoluteGridLength(int value) => AbsoluteGridLength((double)value);
    }
}
