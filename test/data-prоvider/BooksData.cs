using System.Net;
using OnlineBookstore.main.models;
using OnlineBookstore.main.utils;

namespace OnlineBookstore.test.data_prоvider;

public class BooksData : BookHelper
{
    public static IEnumerable<TestCaseData> GetBookWithInvalidData()
    {
        yield return new TestCaseData(GenerateRandomNumber(1000000, 5000000).ToString(), HttpStatusCode.NotFound, "Non-existing ID")
            .SetName("Get Book with Non-existing ID");

        yield return new TestCaseData(GenerateRandomString(3), HttpStatusCode.BadRequest, "Alphabetical ID")
            .SetName("Get Book with Alphabetical ID");

        yield return new TestCaseData(GenerateRandomNumber(-1000, -1).ToString(), HttpStatusCode.BadRequest, "Negative ID")
            .SetName("Get Book with Negative ID");
    }
    
    public static IEnumerable<TestCaseData> CreateBookWithInvalidData()
    {
        yield return new TestCaseData(
            new Book
            {
                Id = GenerateRandomString(15),
                Title = GenerateRandomString(15),
                Description = GenerateRandomString(100),
                PageCount = GenerateRandomNumber(100, 10000).ToString(),
                Excerpt = GenerateRandomString(50),
                PublishDate = GenerateCurrentUtcDate()
            },
            "Invalid ID"
        ).SetName("Create Book with Invalid ID");

        yield return new TestCaseData(
            new Book
            {
                Id = GenerateRandomNumber(1000, 9999).ToString(),
                Title = GenerateRandomString(15),
                Description = GenerateRandomString(100),
                PageCount = GenerateRandomString(15), 
                Excerpt = GenerateRandomString(50),
                PublishDate = GenerateCurrentUtcDate()
            },
            "Invalid PageCount"
        ).SetName("Create Book with Invalid PageCount");

        yield return new TestCaseData(
            new Book
            {
                Id = GenerateRandomNumber(1000, 9999).ToString(),
                Title = GenerateRandomString(15),
                Description = GenerateRandomString(100),
                PageCount = GenerateRandomNumber(100, 10000).ToString(),
                Excerpt = GenerateRandomString(50),
                PublishDate = GenerateRandomString(15) 
            },
            "Invalid PublishDate"
        ).SetName("Create Book with Invalid PublishDate");
    }
    
    public static IEnumerable<TestCaseData> DeleteBookWithInvalidData()
    {
        yield return new TestCaseData(GenerateRandomNumber(1000000, 5000000).ToString(), HttpStatusCode.NotFound, "Non-existing ID")
            .SetName("Delete Book with Non-existing ID");

        yield return new TestCaseData(GenerateRandomString(3), HttpStatusCode.BadRequest, "Alphabetical ID")
            .SetName("Delete Book with Alphabetical ID");

        yield return new TestCaseData(GenerateRandomNumber(-1000, -1).ToString(), HttpStatusCode.BadRequest, "Negative ID")
            .SetName("Delete Book with Negative ID");
    }
}