using Xamarin.Forms;

namespace OnSight
{
	public class BaseContentView<T> : ContentView where T : BaseViewModel
	{
		#region Properties
		protected T ViewModel => GetViewModel();
		#endregion

		#region Methods
		T GetViewModel()
		{
			if (BindingContext is T)
				return BindingContext as T;

			return null;
		}
		#endregion
	}
}
