using System;
using System.IO;

using SQLite;

using Xamarin.Forms;

namespace OnSight
{
    public record PhotoModel
    {
        public PhotoModel() => Id = Guid.NewGuid().ToString();

        [Ignore]
        public ImageSource? ImageSource => GetImageSource();

        [Unique, PrimaryKey]
        public string Id { get; init; }

        public string InspectionModelId { get; init; } = string.Empty;

        public string ImageName { get; init; } = string.Empty;

        public byte[]? Image { get; init; }

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
