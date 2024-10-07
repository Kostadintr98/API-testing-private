using OnlineBookstore.main.models;

namespace OnlineBookstore.main.utils;

public class BookHelper : BaseHelper
{
    protected Book existingBook => GetBookByType("ExistingBook");

    protected Book updateBook => GetBookByType("UpdateBook");

    protected Book deleteBook => GetBookByType("DeleteBook");

    protected static string GenerateCurrentUtcDate()
    {
        DateTime utcNow = DateTime.UtcNow;
        return utcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
    }

    private Book GetBookByType(string bookType)
    {
        return new Book
        {
            Id = _config[$"{bookType}:Id"],
            Title = _config[$"{bookType}:Title"],
            Description = _config[$"{bookType}:Description"],
            PageCount = _config[$"{bookType}:PageCount"],
            Excerpt = _config[$"{bookType}:Excerpt"],
            PublishDate = _config[$"{bookType}:PublishDate"]
        };
    }
    
    protected void VerifyBookData(Book expected, Book actual, string messagePrefix)
    {
        VerifyData(expected.Id, actual.Id, $"{messagePrefix}: Book ID mismatch");
        VerifyData(expected.Title, actual.Title, $"{messagePrefix}: Book Title mismatch");
        VerifyData(expected.Description, actual.Description, $"{messagePrefix}: Book Description mismatch");
        VerifyData(expected.PageCount, actual.PageCount, $"{messagePrefix}: Book PageCount mismatch");
        VerifyData(expected.Excerpt, actual.Excerpt, $"{messagePrefix}: Book Excerpt mismatch");
        VerifyData(expected.PublishDate, actual.PublishDate, $"{messagePrefix}: Book PublishDate mismatch");
    }

    protected Book CreateRandomBook() =>
        new Book
        {
            Id = GenerateRandomNumber(1000, 3999).ToString(),
            Title = GenerateRandomString(15),
            Description = GenerateRandomString(100),
            PageCount = GenerateRandomNumber(100, 10000).ToString(),
            Excerpt = GenerateRandomString(50),
            PublishDate = GenerateCurrentUtcDate()
        };
}