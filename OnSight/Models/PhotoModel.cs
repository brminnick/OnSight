using System;
using System.IO;

using SQLite;

using Xamarin.Forms;

namespace OnSight
{
	public class PhotoModel
	{
		[Unique]
		public string ImageName { get; set; }

		[Ignore]
		public ImageSource Image
		{
			get { return GetImageSource(); }
			set { SaveImageSource(value); }
		}

		string ImageAsBase64String { get; set; }

		#region Methods
		void SaveImageSource(ImageSource image)
		{
			throw new Exception("SaveImageSource Not Implemented");
		}

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
