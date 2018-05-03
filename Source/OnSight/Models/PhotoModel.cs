using System;
using System.IO;

using SQLite;

using Xamarin.Forms;

namespace OnSight
{
    public class PhotoModel
    {
        #region Constructors
        public PhotoModel() => Id = Guid.NewGuid().ToString();
        #endregion

        #region Properties
        [Ignore]
        public ImageSource ImageSource => GetImageSource();

        [Unique, PrimaryKey]
        public string Id { get; set; }

        public string InspectionModelId { get; set; }

        public string ImageName { get; set; }

        public byte[] Image { get; set; }
        #endregion

        #region Methods
        ImageSource GetImageSource()
        {
            try
            {
                if (Image is null)
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
