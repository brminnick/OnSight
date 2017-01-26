using Xamarin.Forms;
namespace OnSight
{
	public class InspectionListPage : BaseContentPage<InspectionListViewModel>
	{
		#region Constructors
		public InspectionListPage()
		{
			var listView = new ListView(ListViewCachingStrategy.RecycleElement)
			{
				ItemTemplate = new DataTemplate(typeof(HSBImageCell))
			};
		}
		#endregion
	}
}
