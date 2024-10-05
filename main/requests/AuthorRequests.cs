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
            _authorsEndpoint = ConfigBuilder.LoadConfiguration()["AuthorsEndpoint"];
        }
        
        public RestResponse GetAllAuthors()
        {
            var getAllAuthors = ExecuteRequest(_authorsEndpoint, Method.Get);
            return getAllAuthors;
        }

        public RestResponse GetAuthorById(string authorId)
        {
            var getAuthorById = ExecuteRequest($"{_authorsEndpoint}/{authorId}", Method.Get);
            return getAuthorById;
        }

        public RestResponse PostNewAuthor()
        {
            var newAuthor = new Author
            {
                Id = int.Parse(_config["NewAuthor:Id"]),
                IdBook = int.Parse(_config["NewAuthor:IdBook"]),
                FirstName = _config["NewAuthor:FirstName"],
                LastName = _config["NewAuthor:LastName"]
            };

            var postNewAuthor = ExecuteRequest(_authorsEndpoint, Method.Post, newAuthor);
            return postNewAuthor;
        }

        public RestResponse PostNewInvalidAuthor()
        {
            var newInvalidAuthor = new InvalidAuthor
            {
                Id = _config["NewInvalidAuthor:Id"],
                IdBook = _config["NewInvalidAuthor:IdBook"],
                FirstName = _config["NewInvalidAuthor:FirstName"],
                LastName = _config["NewInvalidAuthor:LastName"]
            };

            var postNewInvalidAuthor = ExecuteRequest(_authorsEndpoint, Method.Post, newInvalidAuthor);
            return postNewInvalidAuthor;
        }

        public RestResponse UpdateAuthorById(string authorId)
        {
            var updatedAuthor = new Author
            {
                Id = int.Parse(_config["UpdatedAuthor:Id"]),
                IdBook = int.Parse(_config["UpdatedAuthor:IdBook"]),
                FirstName = _config["UpdatedAuthor:FirstName"],
                LastName = _config["UpdatedAuthor:LastName"]
            };

            var updateExistingAuthor = ExecuteRequest($"{_authorsEndpoint}/{authorId}", Method.Put, updatedAuthor);
            return updateExistingAuthor;
        }

        public RestResponse DeleteAuthorById(string authorId)
        {
            var deleteAuthorById = ExecuteRequest($"{_authorsEndpoint}/{authorId}", Method.Delete);
            return deleteAuthorById;
        }
    }
}
