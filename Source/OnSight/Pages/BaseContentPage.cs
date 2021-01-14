using Xamarin.Forms;

namespace OnSight
{
    abstract class BaseContentPage<T> : ContentPage where T : BaseViewModel
    {
        public BaseContentPage(T viewModel) => BindingContext = ViewModel = viewModel;

        public T ViewModel { get; }
    }
}
