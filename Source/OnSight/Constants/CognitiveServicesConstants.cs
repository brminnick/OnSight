using System.Collections.Generic;

namespace OnSight
{
	public static class CognitiveServicesConstants
	{
#error Vision API Key & Base Url Missing. Sign up for a free API Key: https://aka.ms/CognitiveServices/ComputerVisionAPI
        public const string VisionApiKey = "";

        //VisionApi Base Url example: https://westus.api.cognitive.microsoft.com/
        public const string VisionApiBaseUrl = "";

        public static readonly List<string> AcceptablePhotoTags = new List<string>
		{
			"plant",
			"flower"
		};
	}
}
