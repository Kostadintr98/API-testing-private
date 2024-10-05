using Microsoft.Extensions.Configuration;

namespace OnlineBookstore.main.config
{
    public static class ConfigBuilder   
    {
        public static IConfiguration LoadConfiguration()
        {
            var basePath = Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true) 
                .AddJsonFile(Path.Combine("test/resources/data", "authorsTestData.json"), optional: false, reloadOnChange: true)
                .AddJsonFile(Path.Combine("test/resources/data", "booksTestData.json"), optional: false, reloadOnChange: true);
            
            return builder.Build();
        }
    }
}