using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OnlineBookstore.main.config;
using OnlineBookstore.main.models;
using RestSharp;

namespace OnlineBookstore.test.api.steps;

public class BaseSteps
{
    private static Random random = new();
    protected readonly IConfiguration _config;

    protected BaseSteps()
    {
        _config = ConfigBuilder.LoadConfiguration();
    }
    public static void VerifyStatusCode(RestResponse response, HttpStatusCode expectedStatusCode, string errorMessage)
    {
        Assert.That(response.StatusCode, Is.EqualTo(expectedStatusCode), errorMessage);
    }

    public static T DeserializeResponse<T>(RestResponse response)
    {
        return JsonConvert.DeserializeObject<T>(response.Content);
    }

    protected int GenerateRandomNumber(int low, int high)
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