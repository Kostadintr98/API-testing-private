using OnlineBookstore.main.models;

namespace OnlineBookstore.main.utils
{
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
    }
}