using System.Net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OnlineBookstore.main.config;
using OnlineBookstore.main.models;
using OnlineBookstore.main.requests;
using RestSharp;

namespace OnlineBookstore.test.api.tests
{
    public class BooksTests
    {
        private BaseRequests _baseRequest;
        private AuthorRequests _authorRequest;
        private IConfiguration _config;

        [SetUp]
        public void Setup()
        {
            _config = ConfigBuilder.LoadConfiguration(); // Load configuration locally
            _baseRequest = new BaseRequests(_config); // Pass the config to the helper
            _authorRequest = new AuthorRequests(_config);
        }

        [TearDown]
        public void TearDown()
        {
            _baseRequest = null;
        }

        [Test]
        public void GetAllBooks()
        {
            var response = _baseRequest.ExecuteRequest(_config["ApiSettings:BooksEndpoint"], Method.Get);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Failed to retrieve books");

            var books = BaseRequests.DeserializeResponse<List<Book>>(response);
            Console.WriteLine(JsonConvert.SerializeObject(books, Formatting.Indented));
        }

        [Test]
        public void PostBook()
        {
            var newBook = new Book
            {
                Title = _config["NewBook:Title"],
                Description = _config["NewBook:Description"],
                PageCount = int.Parse(_config["NewBook:PageCount"]),
                Excerpt = _config["NewBook:Excerpt"]
            };

            var response = _baseRequest.ExecuteRequest(_config["ApiSettings:BooksEndpoint"], Method.Post, newBook);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Failed to create a book");

            var createdBook = BaseRequests.DeserializeResponse<Book>(response);
            Assert.That(createdBook.Title, Is.EqualTo(_config["NewBook:Title"]));
        }

        [Test]
        public void GetBookById()
        {
            var response = _baseRequest.ExecuteRequest($"{_config["ApiSettings:BooksEndpoint"]}/{_config["BookId"]}", Method.Get);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Failed to retrieve book");

            var book = BaseRequests.DeserializeResponse<Book>(response);
            Console.WriteLine($"Id: {book.Id}, Title: {book.Title}, Description: {book.Description}");
        }

        [Test]
        public void UpdateBookById()
        {
            var updatedBook = new Book
            {
                Id = int.Parse(_config["UpdatedBook:Id"]),
                Title = _config["UpdatedBook:Title"],
                Description = _config["UpdatedBook:Description"],
                PageCount = int.Parse(_config["UpdatedBook:PageCount"]),
                Excerpt = _config["UpdatedBook:Excerpt"]
            };

            var response = _baseRequest.ExecuteRequest($"{_config["ApiSettings:BooksEndpoint"]}/{_config["UpdatedBook:Id"]}", Method.Put, updatedBook);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Failed to update book");

            var updatedResponse = BaseRequests.DeserializeResponse<Book>(response);
            Assert.That(updatedResponse.Title, Is.EqualTo(_config["UpdatedBook:Title"]));
        }

        [Test]
        public void DeleteBookById()
        {
            var response = _baseRequest.ExecuteRequest($"{_config["ApiSettings:BooksEndpoint"]}/{_config["BookId"]}", Method.Delete);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Failed to delete book");
        }
    }
}
