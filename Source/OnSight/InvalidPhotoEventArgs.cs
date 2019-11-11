using System;

namespace OnSight
{
    public class InvalidPhotoEventArgs : EventArgs
    {
        public InvalidPhotoEventArgs(bool doesContainAdultContent, bool doesContainRacyContent, bool doesImageContainAcceptablePhotoTags,
            bool invalidAPIKey, bool internetConnectionFailed)
        {
			DoesContainRacyContent = doesContainRacyContent;
			DoesContainAdultCOntent = doesContainAdultContent;
            DoesImageContainAcceptablePhotoTags = doesImageContainAcceptablePhotoTags;
            InvalidAPIKey = invalidAPIKey;
            InternetConnectionFailed = internetConnectionFailed;
        }

		public bool DoesContainRacyContent { get; }
        public bool DoesContainAdultCOntent { get; }
        public bool DoesImageContainAcceptablePhotoTags { get; }
        public bool InternetConnectionFailed { get; }
        public bool InvalidAPIKey { get; }
    }
}
