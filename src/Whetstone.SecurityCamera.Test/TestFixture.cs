using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whetstone.SecurityCamera.Test
{

    public class TestFixture
    {
        public static IConfiguration InitConfiguration()
        {
            var settingsFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "appsettings.*.json");
            if (settingsFiles.Length != 1) throw new Exception($"Expect to have exactly one configuration-specfic settings file, but found {string.Join(", ", settingsFiles)}.");
            var settingsFile = settingsFiles.First();

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile(settingsFile)
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            return configuration;
        }
    }
}
