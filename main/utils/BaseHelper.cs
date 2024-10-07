using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OnlineBookstore.main.config;
using RestSharp;

namespace OnlineBookstore.main.utils;

/**
* Provides common helper methods for testing and utility purposes.
*
* This class includes methods for verifying data and status codes, deserializing responses, 
* and generating random numbers and strings, which can be used across the application.
*/
public class BaseHelper
{
    private static readonly Random random = new();
    protected readonly IConfiguration _config;

    /**
    * Initializes a new instance of the BaseHelper class.
    *
    * This constructor loads the configuration settings required by derived classes.
    */
    protected BaseHelper()
    {
        _config = ConfigBuilder.LoadConfiguration();
    }

    /**
    * Verifies that the actual data matches the expected data.
    *
    * @param expected The expected data.
    * @param actual The actual data returned from the response.
    * @param message A custom message for the assertion.
    * @typeparam TResponse The type of the data being compared.
    */
    protected static void VerifyData<TResponse>(TResponse expected, TResponse actual, string message)
    {
        Assert.That(actual, Is.EqualTo(expected), message);
        Console.WriteLine(JsonConvert.SerializeObject(actual, Formatting.Indented));
    }

    /**
    * Verifies that the response status code matches the expected status code.
    *
    * @param response The HTTP response returned from the request.
    * @param expectedStatusCode The expected HTTP status code.
    * @param errorMessage A custom error message if the status code does not match.
    */
    protected static void VerifyStatusCode(RestResponse response, HttpStatusCode expectedStatusCode, string errorMessage)
    {
        Assert.That(response.StatusCode, Is.EqualTo(expectedStatusCode), errorMessage);
    }

    /**
    * Verifies the response status code, deserializes the response, and prints the response content.
    *
    * This method checks that the response's status code matches the expected status code and 
    * deserializes the content into the specified type. The deserialized content is printed to the console.
    *
    * @param response The HTTP response to verify.
    * @param expectedStatus The expected HTTP status code.
    * @param errorMessage A custom error message if the status code or content verification fails.
    * @typeparam T The type to deserialize the response content into.
    */
    protected void VerifyAndPrintResponse<T>(RestResponse response, HttpStatusCode expectedStatus, string errorMessage)
    {
        VerifyStatusCode(response, expectedStatus, errorMessage);
        var content = DeserializeResponse<T>(response);
        Assert.IsNotNull(content, errorMessage);
        Console.WriteLine(JsonConvert.SerializeObject(content, Formatting.Indented));
    }

    /**
    * Deserializes the content of a RestResponse into an object of the specified type.
    *
    * @param response The HTTP response containing the data to deserialize.
    * @typeparam TResponse The type of the data to deserialize.
    * @returns TResponse - The deserialized object from the response content.
    */
    protected static TResponse DeserializeResponse<TResponse>(RestResponse response)
    {
        return JsonConvert.DeserializeObject<TResponse>(response.Content);
    }

    /**
    * Generates a random integer between the specified low and high values.
    *
    * @param low The lower bound for the random number.
    * @param high The upper bound for the random number.
    * @returns int - A random integer between the low (inclusive) and high (exclusive) values.
    * @throws ArgumentException If the 'low' parameter is greater than or equal to the 'high' parameter.
    */
    protected static int GenerateRandomNumber(int low, int high)
    {
        if (low >= high)
        {
            throw new ArgumentException("The 'low' parameter must be less than the 'high' parameter.");
        }
        
        return random.Next(low, high);
    }

    /**
    * Generates a random string of the specified length using alphabetic characters.
    *
    * @param length The length of the random string to generate.
    * @returns string - A randomly generated string of the specified length.
    * @throws ArgumentException If the 'length' is less than or equal to zero.
    */
    protected static string? GenerateRandomString(int length)
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
