using System;

namespace OnSight
{
    public class InvalidPhotoEventArgs : EventArgs
    {
        public InvalidPhotoEventArgs(bool isImageInappropriate, bool doesImageContainAcceptablePhotoTags,
            bool invalidAPIKey, bool internetConnectionFailed)
        {
            IsImageInappropriate = isImageInappropriate;
            DoesImageContainAcceptablePhotoTags = doesImageContainAcceptablePhotoTags;
            InvalidAPIKey = invalidAPIKey;
            InternetConnectionFailed = internetConnectionFailed;
        }

        public bool IsImageInappropriate { get; }
        public bool DoesImageContainAcceptablePhotoTags { get; }
        public bool InternetConnectionFailed { get; }
        public bool InvalidAPIKey { get; }
    }
}
