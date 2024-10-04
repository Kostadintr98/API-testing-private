using Microsoft.Extensions.Configuration;

namespace OnlineBookstore.main.config
{
    public static class TestDataBuilder
    {
        public static IConfiguration LoadTestData()
        {
            var basePath = Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(Path.Combine("test/resources/data", "authorsTestData.json"), optional: false, reloadOnChange: true)  // Author test data
                .AddJsonFile(Path.Combine("test/resources/data", "booksTestData.json"), optional: false, reloadOnChange: true);   // Books test data

            return builder.Build();
        }
    }
}