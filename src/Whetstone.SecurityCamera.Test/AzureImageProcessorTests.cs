using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;
using Whetstone.SecurityCamera.AzureImageProcessor;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Whetstone.SecurityCamera.Test
{
    public class AzureImageProcessorTests
    {
        private readonly ITestOutputHelper _output;

        private const string IMAGE_FILE = "00000455_026.jpg";

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

            using (FileStream fs = File.Open(IMAGE_FILE, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    bool isMatched = await azureProcessor.GetTagsAsync(fs, matchList);
                    Assert.True(isMatched);
                }
                finally
                {
                    fs.Close();
                }
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

            using (FileStream fs = File.Open(IMAGE_FILE, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    bool isMatched = await azureProcessor.GetTagsAsync(fs, matchList);
                    Assert.False(isMatched);
                }
                finally
                {
                    fs.Close();
                }
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

            using (FileStream fs = File.Open(IMAGE_FILE, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    await Assert.ThrowsAsync<ComputerVisionErrorResponseException>(async () => await azureProcessor.GetTagsAsync(fs, matchList));
                }
                finally
                {
                    fs.Close();
                }
                
            }
        }

    }
}