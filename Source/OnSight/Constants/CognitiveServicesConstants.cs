using System.Collections.Generic;

namespace OnSight
{
	public static class CognitiveServicesConstants
	{
		//#error Emotion API Key Missing! Sign up for a free API Key
		////Sign up for a free API Key: https://www.microsoft.com/cognitive-services/en-us/computer-vision-api
        public const string VisionAPIKey = "682f3d1f175a4b82b20d79078af0ccae";
		public static readonly List<string> AcceptablePhotoTags = new List<string>
		{
			"plant",
			"flower"
		};
	}
}
