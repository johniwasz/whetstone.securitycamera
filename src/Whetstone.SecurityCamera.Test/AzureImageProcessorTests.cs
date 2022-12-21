using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;
using Whetstone.CameraMonitor.AzureImageProcessor;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Whetstone.CameraMonitor.Test
{
    public class AzureImageProcessorTests
    {
        private readonly ITestOutputHelper _output;

        public AzureImageProcessorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task AzureTestMatchIsGood()
        {
            IConfiguration config = TestFixture.InitConfiguration();

            IImageProcessor azureProcessor = new VisionProcessor(config);

            IEnumerable<KeyValuePair<string, double>> matchList = new List<KeyValuePair<string, double>>
            {
                new KeyValuePair<string, double>("cat", 0.8)
            };

            using (FileStream fs = File.Open("00000455_026.jpg", FileMode.Open))
            {
                bool isMatched = await azureProcessor.GetTagsAsync(fs, matchList);
                Assert.True(isMatched);
            }            
        }

        [Fact]
        public async Task AzureTestMatchIsBad()
        {
            IConfiguration config = TestFixture.InitConfiguration();

            var visionLoggerFactory = LoggerUtilties.GetXUnitLoggerFactory(_output);

            ILogger<VisionProcessor> visionLogger = visionLoggerFactory.CreateLogger<VisionProcessor>();

            IImageProcessor azureProcessor = new VisionProcessor(config);

            IEnumerable<KeyValuePair<string, double>> matchList = new List<KeyValuePair<string, double>>
            {
                new KeyValuePair<string, double>("dog", 0.8)
            };

            using (FileStream fs = File.Open("00000455_026.jpg", FileMode.Open))
            {
                bool isMatched = await azureProcessor.GetTagsAsync(fs, matchList);
                Assert.False(isMatched);
            }
        }


        [Fact]
        public async Task AzureTestAuthenticationFails()
        {
            IConfiguration config = TestFixture.InitConfiguration();

            config[VisionProcessor.CONFIG_SUBSCRIPTION_KEY] = "...";            

            var visionLoggerFactory = LoggerUtilties.GetXUnitLoggerFactory(_output);

            ILogger<VisionProcessor> visionLogger = visionLoggerFactory.CreateLogger<VisionProcessor>();

            IImageProcessor azureProcessor = new VisionProcessor(config);

            IEnumerable<KeyValuePair<string, double>> matchList = new List<KeyValuePair<string, double>>
            {
                new KeyValuePair<string, double>("cat", 0.8)
            };

            using (FileStream fs = File.Open("00000455_026.jpg", FileMode.Open))
            {
                await Assert.ThrowsAsync<ComputerVisionErrorResponseException>(async () => await azureProcessor.GetTagsAsync(fs, matchList));

                
            }
        }

    }
}