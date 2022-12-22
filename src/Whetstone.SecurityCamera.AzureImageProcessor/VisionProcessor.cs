using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Net;


namespace Whetstone.SecurityCamera.AzureImageProcessor
{
    public class VisionProcessor : IImageProcessor
    {
        public const string CONFIG_SUBSCRIPTION_KEY = "VisionSubscriptionKey";
        public const string CONFIG_VISION_ENDPOINT = "VisionEndpoint";

        private readonly ComputerVisionClient _visionClient;
        
        public VisionProcessor(IConfiguration config)
        {

            string subscriptionKey = config[CONFIG_SUBSCRIPTION_KEY];
            if(string.IsNullOrWhiteSpace(subscriptionKey) ) 
            {
                throw new ArgumentException("Configuration cannot be null or empty", CONFIG_SUBSCRIPTION_KEY);
            }

            string visionEndpoint = config[CONFIG_VISION_ENDPOINT];
            if(string.IsNullOrEmpty(visionEndpoint) ) 
            {
                throw new ArgumentException("Configuration be null or empty", CONFIG_VISION_ENDPOINT);
            }

            if (!Uri.TryCreate(visionEndpoint, new UriCreationOptions(), out Uri? visionUri))
            {
                throw new ArgumentException($"Configuration setting value is not a valid Uri {visionEndpoint}", CONFIG_VISION_ENDPOINT);
            }

            _visionClient = Authenticate(visionUri, subscriptionKey);
        }


        public async Task<bool> GetTagsAsync(Stream imageStream, IEnumerable<KeyValuePair<string, double>> minimumMatches)
        {

            if (imageStream is null)
            {
                throw new ArgumentNullException(nameof(imageStream));
            }

            if (minimumMatches is null)
            {
                throw new ArgumentNullException(nameof(minimumMatches));
            }

            if (!minimumMatches.Any())
            {
                throw new ArgumentException("Cannot be empty", nameof(minimumMatches));
            }

            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Tags
            };

            bool isMatchMissed = false;

            ImageAnalysis results = await _visionClient.AnalyzeImageInStreamAsync(imageStream, visualFeatures: features);

            List<KeyValuePair<string, double>> keyList = minimumMatches.ToList();

            int index = 0;

            // Iterate of the requested minimumMatches
            // Exit when the the first match is missed
            while (index < keyList.Count && !isMatchMissed)
            {
                var matchItem = keyList[index];
                double confidence = 0d;

                ImageTag? foundTag = results.Tags.SingleOrDefault(x => x.Name.Equals(matchItem.Key, StringComparison.OrdinalIgnoreCase));

                if (foundTag != null)
                {
                    confidence = foundTag.Confidence;
                }

                index++;
                isMatchMissed = confidence < matchItem.Value;

            }


            return !isMatchMissed;
        }

        private ComputerVisionClient Authenticate(Uri endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint.ToString() };
            return client;
        }

    }
}