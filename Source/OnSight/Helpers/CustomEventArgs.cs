using System;
namespace OnSight
{
	public class InvalidPhotoEventArgs : EventArgs
	{
		readonly bool _isImageInappropriate, _doesImageContainAcceptablePhotoTags, _invalidAPIKey, _internetConnectionFailed;

		public InvalidPhotoEventArgs(
			bool isImageInappropriate,
			bool doesImageContainAcceptablePhotoTags,
			bool invalidAPIKey,
			bool internetConnectionFailed)
		{
			_isImageInappropriate = isImageInappropriate;
			_doesImageContainAcceptablePhotoTags = doesImageContainAcceptablePhotoTags;
			_invalidAPIKey = invalidAPIKey;
			_internetConnectionFailed = internetConnectionFailed;
		}

		public bool IsImageInappropriate => _isImageInappropriate;
		public bool DoesImageContainAcceptablePhotoTags => _doesImageContainAcceptablePhotoTags;
		public bool InternetConnectionFailed => _internetConnectionFailed;
		public bool InvalidAPIKey => _invalidAPIKey;
	}
}
