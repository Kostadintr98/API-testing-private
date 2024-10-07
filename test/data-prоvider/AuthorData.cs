using System.Net;
using OnlineBookstore.main.models;
using OnlineBookstore.main.utils;

namespace OnlineBookstore.test.data_prоvider;

public class AuthorData : AuthorHelper
{

    public static IEnumerable<TestCaseData> GetAuthorWithInvalidData()
    {
        yield return new TestCaseData(GenerateRandomNumber(1000000, 5000000).ToString(), HttpStatusCode.NotFound, "Non-existing ID")
            .SetName("Get Author with Non-existing ID");

        yield return new TestCaseData(GenerateRandomString(3), HttpStatusCode.BadRequest, "Alphabetical ID")
            .SetName("Get Author with Alphabetical ID");

        yield return new TestCaseData(GenerateRandomNumber(-1000, -1).ToString(), HttpStatusCode.BadRequest, "Negative ID")
            .SetName("Get Author with Negative ID");
    }
    
    public static IEnumerable<TestCaseData> CreateAuthorWithInvalidData()
    {
        yield return new TestCaseData(
            new Author
            {
                Id = GenerateRandomString(15),
                IdBook = GenerateRandomNumber(4000, 7999).ToString(),
                FirstName = GenerateRandomString(15),
                LastName = GenerateRandomString(15)
            },
            "Invalid ID"
        ).SetName("Create Author with Invalid ID");

        yield return new TestCaseData(
            new Author
            {
                Id = GenerateRandomNumber(1000, 3999).ToString(),
                IdBook = GenerateRandomString(15),  
                FirstName = GenerateRandomString(15),
                LastName = GenerateRandomString(15)
            },
            "Invalid IdBook"
        ).SetName("Create Author with Invalid IdBook");
    }
    
    public static IEnumerable<TestCaseData> DeleteAuthorWithInvalidData()
    {
        yield return new TestCaseData(GenerateRandomNumber(1000000, 5000000).ToString(), HttpStatusCode.NotFound, "Non-existing ID")
            .SetName("Delete Author with Non-existing ID");

        yield return new TestCaseData(GenerateRandomString(3), HttpStatusCode.BadRequest, "Alphabetical ID")
            .SetName("Delete Author with Alphabetical ID");

        yield return new TestCaseData(GenerateRandomNumber(-1000, -1).ToString(), HttpStatusCode.BadRequest, "Negative ID")
            .SetName("Delete Author with Negative ID");
    }
}