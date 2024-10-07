using Microsoft.Extensions.Configuration;
using OnlineBookstore.main.config;
using RestSharp;

namespace OnlineBookstore.main.requests;

/**
* Provides a base class for making HTTP requests using the RestSharp client.
*
* This class offers common functionality for making HTTP requests to the API, 
* including the creation of a RestClient and methods to execute different types of requests.
*/
public class BaseRequests
{
    private readonly IConfiguration _config;
    private readonly RestClient _client;

    /**
    * Initializes a new instance of the BaseRequests class.
    *
    * This constructor loads the configuration and initializes the RestClient with the base URL for the API.
    */
    protected BaseRequests()
    {
        _config = ConfigBuilder.LoadConfiguration();
        _client = CreateRestClient();
    }

    /**
    * Creates and configures a RestClient using the base URL from the configuration.
    *
    * @returns RestClient - The configured RestClient instance.
    */
    private RestClient CreateRestClient()
    {
        var baseUrl = _config["API:BaseUrl"];
        return baseUrl != null ? new RestClient(new RestClientOptions { BaseUrl = new Uri(baseUrl) }) : new RestClient(new RestClientOptions());
    }

    /**
    * Creates a new RestRequest for a specified endpoint and HTTP method.
    *
    * @param endpoint The API endpoint to target.
    * @param method The HTTP method for the request (default is GET).
    * @returns RestRequest - The created request object.
    */
    private static RestRequest CreateRequest(string? endpoint, Method method = Method.Get)
    {
        return new RestRequest(endpoint, method);
    }

    /**
    * Executes an HTTP request for the specified endpoint, method, and optional request body.
    *
    * This method supports GET, POST, PUT, and DELETE methods and can include a request body if provided.
    *
    * @param endpoint The API endpoint to target.
    * @param method The HTTP method to use (GET, POST, PUT, DELETE).
    * @param body The optional request body to include (default is null).
    * @returns RestResponse - The response received from the server.
    */
    protected RestResponse ExecuteRequest(string? endpoint, Method method, object body = null!)
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
}
