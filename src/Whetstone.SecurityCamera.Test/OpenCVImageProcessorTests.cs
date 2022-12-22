using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whetstone.SecurityCamera.AzureImageProcessor;
using Whetstone.SecurityCamera.OpenCV;
using Xunit.Abstractions;

namespace Whetstone.SecurityCamera.Test
{
    public class OpenCVImageProcessorTests
    {
        private readonly ITestOutputHelper _output;

        public OpenCVImageProcessorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task AzureTestMatchIsGood()
        {
            IConfiguration config = TestFixture.InitConfiguration();

            IImageProcessor azureProcessor = new ImageProcessorOpenCV();

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

    }
}
