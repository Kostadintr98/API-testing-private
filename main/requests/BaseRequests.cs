using Microsoft.Extensions.Configuration;
using OnlineBookstore.main.config;
using RestSharp;

namespace OnlineBookstore.main.requests;

public class BaseRequests
{
    private readonly IConfiguration _config;
    private readonly RestClient _client;

    protected BaseRequests()
    {
        _config = ConfigBuilder.LoadConfiguration();
        _client = CreateRestClient();
    }
    
    private RestClient CreateRestClient()
    {
        var baseUrl = _config["API:BaseUrl"];
        return new RestClient(new RestClientOptions { BaseUrl = new Uri(baseUrl) });
    }
    
    private static RestRequest CreateRequest(string? endpoint, Method method = Method.Get)
    {
        return new RestRequest(endpoint, method);
    }

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