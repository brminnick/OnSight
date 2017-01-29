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

		public string ImageAsBase64String { get; set; }
		#endregion

		#region Methods
		ImageSource GetImageSource()
		{
			try
			{
				if (string.IsNullOrEmpty(ImageAsBase64String))
					return null;

				var imageByteArray = Convert.FromBase64String(ImageAsBase64String);

				return ImageSource.FromStream(() => new MemoryStream(imageByteArray));
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
