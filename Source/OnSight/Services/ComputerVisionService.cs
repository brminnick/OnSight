using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Linq;
using System.Net.Http;
using System.Net;

namespace OnSight
{
	static class ComputerVisionService
	{
		#region Constant Fields
		static Lazy<ComputerVisionAPI> _computerVisionApiClientHolder =
			new Lazy<ComputerVisionAPI>(() => new ComputerVisionAPI(new ApiKeyServiceClientCredentials(CognitiveServicesConstants.VisionAPIKey)) { AzureRegion = Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models.AzureRegions.Westus });
		#endregion

		#region Events
		public static event EventHandler<InvalidPhotoEventArgs> DisplayInvalidPhotoAlert;
		#endregion

		#region Properties
		static ComputerVisionAPI ComputerVisionApiClient => _computerVisionApiClientHolder.Value;
		#endregion

		#region Methods
		public static async Task<bool> IsPhotoValid(Stream photo, List<string> acceptablePhotoTags, bool shouldAllowAdultContent = false, bool shouldAllowRacyContent = false)
		{
			bool doesImageContainAcceptablePhotoTags;
			bool isInvalidAPIKey = false, hasInternetConnectionFailed = false;

			ImageAnalysis imageAnalysisResult;
			try
			{
				imageAnalysisResult = await ComputerVisionApiClient.AnalyzeImageInStreamAsync(photo, new List<VisualFeatureTypes> { VisualFeatureTypes.Adult, VisualFeatureTypes.Description }).ConfigureAwait(false);
			}
			catch (HttpRequestException e) when (((e?.InnerException as WebException)?.Status.Equals(WebExceptionStatus.NameResolutionFailure) ?? false)
												 || ((e?.InnerException as WebException)?.Status.Equals(WebExceptionStatus.ConnectFailure) ?? false))
			{
				DebugHelpers.PrintException(e);

				imageAnalysisResult = null;
				hasInternetConnectionFailed = true;
			}
			catch (ComputerVisionErrorException e) when (e.Response.StatusCode.Equals(HttpStatusCode.Unauthorized))
			{
				DebugHelpers.PrintException(e);

				imageAnalysisResult = null;
				isInvalidAPIKey = true;
			}

			var doesContainAdultContent = imageAnalysisResult?.Adult?.IsAdultContent ?? false;
			var doesContainRacyContent = imageAnalysisResult?.Adult?.IsRacyContent ?? false;

			doesImageContainAcceptablePhotoTags = imageAnalysisResult?.Description?.Tags?.Intersect(acceptablePhotoTags)?.Any() ?? false;

			if ((doesContainAdultContent && !shouldAllowAdultContent)
				|| (doesContainRacyContent && !shouldAllowRacyContent)
				|| !doesImageContainAcceptablePhotoTags
				|| isInvalidAPIKey
				|| hasInternetConnectionFailed)
			{
				OnDisplayInvalidPhotoAlert(doesContainAdultContent, doesContainRacyContent, doesImageContainAcceptablePhotoTags, isInvalidAPIKey, hasInternetConnectionFailed);
				return false;
			}

			return true;
		}

		static void OnDisplayInvalidPhotoAlert(
			bool doesContainAdultContent,
			bool doesContainRacyContent,
			bool doesImageContainAcceptablePhotoTags,
			bool invalidAPIKey,
			bool internetConnectionFailed) =>
		DisplayInvalidPhotoAlert?.Invoke(null, new InvalidPhotoEventArgs(doesContainAdultContent, doesContainRacyContent, doesImageContainAcceptablePhotoTags, invalidAPIKey, internetConnectionFailed));
		#endregion
	}
}
