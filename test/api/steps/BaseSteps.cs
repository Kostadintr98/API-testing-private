using System.Net;
using System.Text;
using RestSharp;
using Newtonsoft.Json;
using OnlineBookstore.main.config;
using Microsoft.Extensions.Configuration;

namespace OnlineBookstore.test.api.steps;

public class BaseSteps
{
    private static readonly Random random = new();
    protected readonly IConfiguration _config;

    protected BaseSteps()
    {
        _config = ConfigBuilder.LoadConfiguration();
    }

    protected static void VerifyData<TResponse>(TResponse expected, TResponse actual, string message)
    {
        Assert.That(actual, Is.EqualTo(expected), message);
        Console.WriteLine(JsonConvert.SerializeObject(actual, Formatting.Indented));
    }

    protected static void VerifyStatusCode(RestResponse response, HttpStatusCode expectedStatusCode, string errorMessage)
    {
        Assert.That(response.StatusCode, Is.EqualTo(expectedStatusCode), errorMessage);
    }

    protected static TResponse DeserializeResponse<TResponse>(RestResponse response)
    {
        return JsonConvert.DeserializeObject<TResponse>(response.Content);
    }

    protected static int GenerateRandomNumber(int low, int high)
    {
        if (low >= high)
        {
            throw new ArgumentException("The 'low' parameter must be less than the 'high' parameter.");
        }
        
        return random.Next(low, high);
    }

    protected static string GenerateRandomString(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentException("Length must be greater than zero.");
        }

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        var result = new StringBuilder(length);

        for (var i = 0; i < length; i++)
        {
            var randomChar = chars[random.Next(chars.Length)];
            result.Append(randomChar);
        }

        return result.ToString();
    }
}