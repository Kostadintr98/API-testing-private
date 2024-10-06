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
    private readonly RestClient _client;
    public readonly IConfiguration _config;
        
    public BaseSteps()
    {
        _config = ConfigBuilder.LoadConfiguration();
        _client = CreateRestClient();
    }

    private Author GetAuthorByType(string authorType)
    {
        return new Author
        {
            Id = _config[$"{authorType}:Id"],
            IdBook = _config[$"{authorType}:IdBook"],
            FirstName = _config[$"{authorType}:FirstName"],
            LastName = _config[$"{authorType}:LastName"]
        };
    }

    protected Author existingAuthor => GetAuthorByType("ExistingAuthor");
    protected Author updateAuthor => GetAuthorByType("UpdateAuthor");
    protected Author deleteAuthor => GetAuthorByType("DeleteAuthor");


    private RestClient CreateRestClient()
    {
        var baseUrl = _config["API:BaseUrl"];
        return new RestClient(new RestClientOptions { BaseUrl = new Uri(baseUrl) });
    }

    public RestResponse ExecuteRequest(string endpoint, Method method, object body = null!)
    {
        var request = CreateRequest(endpoint, method);
        if (body != null!)
        {
            request.AddJsonBody(body);
        }

        return method switch
        {
            Method.Post => _client.Post(request),
            Method.Put => _client.Put(request),
            Method.Delete => _client.Delete(request),
            _ => _client.Get(request)
        };
    }
        
    private static RestRequest CreateRequest(string endpoint, Method method = Method.Get)
    {
        return new RestRequest(endpoint, method);
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