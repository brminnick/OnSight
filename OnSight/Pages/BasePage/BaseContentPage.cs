using Xamarin.Forms;

namespace OnSight
{
	public abstract class BaseContentPage<T> : ContentPage where T : BaseViewModel, new()
	{
		#region Constructors
		protected BaseContentPage()
		{
			BindingContext = new T();
		}
		#endregion

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
