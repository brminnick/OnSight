using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Linq;
using System.Net.Http;
using System.Net;
using AsyncAwaitBestPractices;

namespace OnSight
{
    static class ComputerVisionService
    {
        static readonly WeakEventManager<InvalidPhotoEventArgs> _invalidPhotoSubmittedEventManager = new WeakEventManager<InvalidPhotoEventArgs>();

        static readonly Lazy<ComputerVisionClient> _computerVisionApiClientHolder =
            new Lazy<ComputerVisionClient>(() => new ComputerVisionClient(new ApiKeyServiceClientCredentials(CognitiveServicesConstants.VisionApiKey)) { Endpoint = CognitiveServicesConstants.VisionApiBaseUrl });

        public static event EventHandler<InvalidPhotoEventArgs> InvalidPhotoSubmitted
        {
            add => _invalidPhotoSubmittedEventManager.AddEventHandler(value);
            remove => _invalidPhotoSubmittedEventManager.RemoveEventHandler(value);
        }

        static ComputerVisionClient ComputerVisionApiClient => _computerVisionApiClientHolder.Value;

        public static async Task<bool> IsPhotoValid(Stream photo, List<string> acceptablePhotoTags, bool shouldAllowAdultContent = false, bool shouldAllowRacyContent = false)
        {
            bool doesImageContainAcceptablePhotoTags;
            bool isInvalidAPIKey = false, hasInternetConnectionFailed = false;

            ImageAnalysis? imageAnalysisResult;
            try
            {
                imageAnalysisResult = await ComputerVisionApiClient.AnalyzeImageInStreamAsync(photo, new List<VisualFeatureTypes> { VisualFeatureTypes.Adult, VisualFeatureTypes.Description }).ConfigureAwait(false);
            }
            catch (HttpRequestException e) when (e.InnerException is WebException webException
                                                    && (webException.Status.Equals(WebExceptionStatus.NameResolutionFailure)
                                                        || (webException.Status.Equals(WebExceptionStatus.ConnectFailure))))
            {
                DebugService.PrintException(e);

                imageAnalysisResult = null;
                hasInternetConnectionFailed = true;
            }
            catch (ComputerVisionErrorException e) when (e.Response.StatusCode.Equals(HttpStatusCode.Unauthorized))
            {
                DebugService.PrintException(e);

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
        _invalidPhotoSubmittedEventManager.HandleEvent(null, new InvalidPhotoEventArgs(doesContainAdultContent, doesContainRacyContent, doesImageContainAcceptablePhotoTags, invalidAPIKey, internetConnectionFailed), nameof(InvalidPhotoSubmitted));
    }
}
