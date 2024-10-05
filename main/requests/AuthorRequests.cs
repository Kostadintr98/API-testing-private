using OnlineBookstore.main.config;
using RestSharp;
using OnlineBookstore.main.models;

namespace OnlineBookstore.main.requests
{
    public class AuthorRequests : BaseRequests
    {
        
        private static readonly string? _authorsEndpoint;

        // Static constructor to initialize the constant
        static AuthorRequests()
        {
            _authorsEndpoint = ConfigBuilder.LoadConfiguration()["API:AuthorsEndpoint"];
        }
        
        public RestResponse GetAllAuthors()
        {
            return ExecuteRequest(_authorsEndpoint, Method.Get);
        }

        public RestResponse GetAuthorById(string authorId)
        {
            return ExecuteRequest($"{_authorsEndpoint}/{authorId}", Method.Get);
        }
        
        public RestResponse PostNewAuthor(Author author)
        {
            return ExecuteRequest(_authorsEndpoint, Method.Post, author);
        }

        public RestResponse PostNewInvalidAuthor(Author author)
        {
            return ExecuteRequest(_authorsEndpoint, Method.Post, author);
        }

        public RestResponse UpdateAuthorById(string authorId, Author author)
        {
            return ExecuteRequest($"{_authorsEndpoint}/{authorId}", Method.Put, author);
        }

        public RestResponse DeleteAuthorById(string authorId)
        {
            return ExecuteRequest($"{_authorsEndpoint}/{authorId}", Method.Delete);
        }
    }
}
