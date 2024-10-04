using RestSharp;
using OnlineBookstore.main.models;

namespace OnlineBookstore.main.requests
{
    public class AuthorRequests : BaseRequests
    {
        public AuthorRequests() : base() { }

        public RestResponse GetAllAuthors()
        {
            var getAllAuthors = ExecuteRequest(_config["ApiSettings:AuthorsEndpoint"], Method.Get);
            return getAllAuthors;
        }

        public RestResponse GetAuthorById(string authorId)
        {
            var getAuthorById = ExecuteRequest($"{_config["ApiSettings:AuthorsEndpoint"]}/{authorId}", Method.Get);
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

            var postNewAuthor = ExecuteRequest(_config["ApiSettings:AuthorsEndpoint"], Method.Post, newAuthor);
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

            var postNewInvalidAuthor = ExecuteRequest(_config["ApiSettings:AuthorsEndpoint"], Method.Post, newInvalidAuthor);
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

            var updateExistingAuthor = ExecuteRequest($"{_config["ApiSettings:AuthorsEndpoint"]}/{authorId}", Method.Put, updatedAuthor);
            return updateExistingAuthor;
        }

        public RestResponse DeleteAuthorById(string authorId)
        {
            var deleteAuthorById = ExecuteRequest($"{_config["ApiSettings:AuthorsEndpoint"]}/{authorId}", Method.Delete);
            return deleteAuthorById;
        }
    }
}
