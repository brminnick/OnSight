using System.Collections.Generic;

namespace OnSight
{
	public static class CognitiveServicesConstants
	{
		//#error Emotion API Key Missing! Sign up for a free API Key
		////Sign up for a free API Key: https://www.microsoft.com/cognitive-services/en-us/computer-vision-api
		public const string VisionAPIKey = "10e99346adf64a9482f293bf9c455acc";
		public static readonly List<string> AcceptablePhotoTags = new List<string>
		{
			"plant",
			"flower"
		};
	}
}
