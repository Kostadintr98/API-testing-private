using System.Net;
using Newtonsoft.Json;
using OnlineBookstore.main.requests;
using OnlineBookstore.main.models;
using RestSharp;

namespace OnlineBookstore.test.api.tests
{
    public class BooksTests
    {
        private BaseRequests _baseRequest;

        [SetUp]
        public void Setup()
        {
            _baseRequest = new BaseRequests();
        }

        [TearDown]
        public void TearDown()
        {
            _baseRequest = null;
        }

        [Test]
        public void GetAllBooks()
        {
            var response = _baseRequest.ExecuteRequest(_baseRequest._config["ApiSettings:BooksEndpoint"], Method.Get);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Failed to retrieve books");

            var books = BaseRequests.DeserializeResponse<List<Book>>(response);
            Console.WriteLine(JsonConvert.SerializeObject(books, Formatting.Indented));

            Assert.IsNotNull(books, "Books list is null");
            Assert.IsTrue(books.Count > 0, "Books list is empty");
        }

        [Test]
        public void PostBook()
        {
            var newBook = new Book
            {
                Title = _baseRequest._config["NewBook:Title"],
                Description = _baseRequest._config["NewBook:Description"],
                PageCount = int.Parse(_baseRequest._config["NewBook:PageCount"]),
                Excerpt = _baseRequest._config["NewBook:Excerpt"]
            };

            var response = _baseRequest.ExecuteRequest(_baseRequest._config["ApiSettings:BooksEndpoint"], Method.Post, newBook);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Failed to create a book");

            var createdBook = BaseRequests.DeserializeResponse<Book>(response);
            Assert.That(createdBook.Title, Is.EqualTo(_baseRequest._config["NewBook:Title"]));
        }

        [Test]
        public void GetBookById()
        {
            var response = _baseRequest.ExecuteRequest($"{_baseRequest._config["ApiSettings:BooksEndpoint"]}/{_baseRequest._config["BookId"]}", Method.Get);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Failed to retrieve book");

            var book = BaseRequests.DeserializeResponse<Book>(response);
            Console.WriteLine($"Id: {book.Id}, Title: {book.Title}, Description: {book.Description}");

            Assert.IsNotNull(book, "Book is null");
            Assert.AreEqual(_baseRequest._config["BookId"], book.Id.ToString(), "Book ID does not match");
        }

        [Test]
        public void UpdateBookById()
        {
            var updatedBook = new Book
            {
                Id = int.Parse(_baseRequest._config["UpdatedBook:Id"]),
                Title = _baseRequest._config["UpdatedBook:Title"],
                Description = _baseRequest._config["UpdatedBook:Description"],
                PageCount = int.Parse(_baseRequest._config["UpdatedBook:PageCount"]),
                Excerpt = _baseRequest._config["UpdatedBook:Excerpt"]
            };

            var response = _baseRequest.ExecuteRequest($"{_baseRequest._config["ApiSettings:BooksEndpoint"]}/{_baseRequest._config["UpdatedBook:Id"]}", Method.Put, updatedBook);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Failed to update book");

            var updatedResponse = BaseRequests.DeserializeResponse<Book>(response);
            Assert.That(updatedResponse.Title, Is.EqualTo(_baseRequest._config["UpdatedBook:Title"]));
        }

        [Test]
        public void DeleteBookById()
        {
            var response = _baseRequest.ExecuteRequest($"{_baseRequest._config["ApiSettings:BooksEndpoint"]}/{_baseRequest._config["BookId"]}", Method.Delete);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Failed to delete book");

            // Optionally verify the book has been deleted by attempting to retrieve it
            response = _baseRequest.ExecuteRequest($"{_baseRequest._config["ApiSettings:BooksEndpoint"]}/{_baseRequest._config["BookId"]}", Method.Get);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.NotFound, "Book was not deleted successfully");
        }
    }
}
