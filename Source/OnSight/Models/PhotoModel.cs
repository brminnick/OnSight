using System;
using System.IO;

using SQLite;

using Xamarin.Forms;

namespace OnSight
{
	public class PhotoModel
	{
		#region Properties
		[Ignore]
		public ImageSource ImageSource => GetImageSource();

		[Unique, AutoIncrement, PrimaryKey]
		public int Id { get; set; }

		public int InspectionModelId { get; set; }

		public string ImageName { get; set; }

		public byte[] Image { get; set; }
		#endregion

		#region Methods
		ImageSource GetImageSource()
		{
			try
			{
                if (Image == null)
					return null;

				return ImageSource.FromStream(() => new MemoryStream(Image));
			}
			catch (Exception e)
			{
				DebugHelpers.PrintException(e);
				return null;
			}
		}
		#endregion
	}
}
