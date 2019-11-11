using Xamarin.Forms;

namespace OnSight
{
    public class BaseContentView<T> : ContentView where T : BaseViewModel, new()
    {
        protected T ViewModel { get; } = new T();
    }
}
