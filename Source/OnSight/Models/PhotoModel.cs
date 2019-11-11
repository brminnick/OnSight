using System;
using System.IO;

using SQLite;

using Xamarin.Forms;

namespace OnSight
{
    public class PhotoModel
    {
        public PhotoModel() => Id = Guid.NewGuid().ToString();

        [Ignore]
        public ImageSource? ImageSource => GetImageSource();

        [Unique, PrimaryKey]
        public string Id { get; set; }

        public string InspectionModelId { get; set; } = string.Empty;

        public string ImageName { get; set; } = string.Empty;

        public byte[]? Image { get; set; }

        ImageSource? GetImageSource()
        {
            try
            {
                if (Image is null)
                    return null;

                return ImageSource.FromStream(() => new MemoryStream(Image));
            }
            catch (Exception e)
            {
                DebugService.PrintException(e);
                return null;
            }
        }
    }
}
