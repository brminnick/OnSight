using System;
using System.IO;

using SQLite;
using Xamarin.Forms;

namespace OnSight
{
	public class PhotoModel
	{
		[Ignore]
		public Stream ImageStream
		{
			get { return GetImageStream(); }
			set { SaveImageStream(value); }
		}

		public int InspectionModelId { get; set; }

		public string ImageName { get; set; }

		string ImageAsBase64String { get; set; }

		#region Methods
		void SaveImageStream(Stream image)
		{
			using (var memoryStream = new MemoryStream())
			{
				image.CopyTo(memoryStream);
				var imageByteArray = memoryStream.ToArray();
				ImageAsBase64String = Convert.ToBase64String(imageByteArray);
			}
		}

		Stream GetImageStream()
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
