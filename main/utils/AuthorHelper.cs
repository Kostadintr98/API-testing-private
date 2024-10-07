using OnlineBookstore.main.models;

namespace OnlineBookstore.main.utils;

public class AuthorHelper : BaseHelper
{
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

    protected void VerifyAuthorData(Author expected, Author actual, string messagePrefix)
    {
        VerifyData(expected.Id, actual.Id, $"{messagePrefix}: Author ID mismatch");
        VerifyData(expected.IdBook, actual.IdBook, $"{messagePrefix}: Author BookID mismatch");
        VerifyData(expected.FirstName, actual.FirstName, $"{messagePrefix}: Author First Name mismatch");
        VerifyData(expected.LastName, actual.LastName, $"{messagePrefix}: Author Last Name mismatch");
    }
}