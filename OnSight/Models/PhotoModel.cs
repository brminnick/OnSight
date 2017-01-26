using System;
using System.IO;

using SQLite;

namespace OnSight
{
	public class PhotoModel
	{
		[Unique]
		public string ImageName { get; set; }

		[Ignore]
		public Stream ImageStream
		{
			get { return GetImageSource(); }
			set { SaveImageSource(value); }
		}

		string ImageAsBase64String { get; set; }

		#region Methods
		void SaveImageSource(Stream image)
		{
			using (var memoryStream = new MemoryStream())
			{
				image.CopyTo(memoryStream);
				var imageByteArray = memoryStream.ToArray();
				ImageAsBase64String = Convert.ToBase64String(imageByteArray);
			}
		}

		Stream GetImageSource()
		{
			try
			{
				if (string.IsNullOrEmpty(ImageAsBase64String))
					return null;

				var imageByteArray = Convert.FromBase64String(ImageAsBase64String);

				return new MemoryStream(imageByteArray);
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
