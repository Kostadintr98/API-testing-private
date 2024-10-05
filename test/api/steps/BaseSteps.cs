using System.Text;

namespace OnlineBookstore.test.api.steps;

public class BaseSteps
{
    private static Random random = new Random();

    public int GenerateRandomNumber(int low, int high)
    {
        // Ensure low is less than high
        if (low >= high)
        {
            throw new ArgumentException("The 'low' parameter must be less than the 'high' parameter.");
        }

        // Generate a random number in the specified range
        return random.Next(low, high);
    }

    public static string GenerateRandomString(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentException("Length must be greater than zero.");
        }

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        StringBuilder result = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            // Select a random character from the defined set
            char randomChar = chars[random.Next(chars.Length)];
            result.Append(randomChar);
        }

        return result.ToString();
    }
}