using System.Net;
using Allure.NUnit;
using Newtonsoft.Json;
using OnlineBookstore.main.requests;
using OnlineBookstore.main.models;
using OnlineBookstore.main.utils;
using RestSharp;
using Exception = System.Exception;

namespace OnlineBookstore.test.api.tests
{
    [AllureNUnit]
    public class BookTests : BookHelper
    {
        private BookRequests _bookRequest;

        [SetUp]
        public void Setup()
        {
            _bookRequest = new BookRequests();
        }

        [Test(Description = "Can Get all Books")]
        public void GetAllBooks()
        {
            var response = _bookRequest.GetAllBooks();
            VerifyAndPrintResponse<List<Book>>(response, HttpStatusCode.OK, "Failed to retrieve books");
        }

        [Test(Description = "Can Get Book by existing ID")]
        public void GetBookById()
        {
            var response = _bookRequest.GetBookById(existingBook.Id);
            var book = DeserializeResponse<Book>(response);
            VerifyBookData(existingBook, book, "Book retrieval mismatch");
        }

        [Test(Description = "Can Get Book with non-existing positive ID")]
        public void GetBookByNonExistingId()
        {
            var bookId = GenerateRandomNumber(1000000, 5000000).ToString();
            var response = _bookRequest.GetBookById(bookId);
            VerifyStatusCode(response, HttpStatusCode.NotFound, $"Book with ID {bookId} found unexpectedly");
        }

        [Test(Description = "Can not Get Book with alphabetical ID")]
        public void GetBookByAlphabeticalId()
        {
            Assert.Throws<HttpRequestException>(() =>
            {
                var bookId = GenerateRandomString(3);
                var response = _bookRequest.GetBookById(bookId);
                VerifyStatusCode(response, HttpStatusCode.BadRequest, $"Request failed with status code: {response.StatusCode}");
                Console.WriteLine($"Book with ID: {bookId} have not been found.");
            });
        }

        [Test(Description = "Can not Get Book with negative number ID")]
        public void GetBookByNegativeNumberId()
        {
            var bookId = GenerateRandomNumber(-1000, -1).ToString();
            var response = _bookRequest.GetBookById(bookId);
            VerifyStatusCode(response, HttpStatusCode.NotFound, $"Request failed with status code: {response.StatusCode}");
        }

        [Test(Description = "Can Create a new Book")]
        public void CreateNewBook()
        {
            var newBook = CreateRandomBook();
            var response = _bookRequest.PostNewBook(newBook);
            var createdBook = DeserializeResponse<Book>(response);
            VerifyBookData(newBook, createdBook, "Created book mismatch");
        }

        [Test(Description = "Can not Create a new Book with invalid ID")]
        public void CreateNewBookWithInvalidId()
        {
            try
            {
                var newBook = new Book
                {
                    Id = GenerateRandomString(15),
                    Title = GenerateRandomString(15),
                    Description = GenerateRandomString(100),
                    PageCount = GenerateRandomNumber(100, 10000).ToString(),
                    Excerpt = GenerateRandomString(50),
                    PublishDate = GenerateCurrentUtcDate()
                };
                var response = _bookRequest.PostNewInvalidBook(newBook);
                VerifyStatusCode(response, HttpStatusCode.BadRequest, "Unexpectedly succeeded in creating an invalid book");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                Console.WriteLine("Book with given ID have not been created.");
            }
        }

        [Test(Description = "Can not Create a new Book with invalid PageCount")]
        public void CreateNewBookWithInvalidPageCount()
        {
            try
            {
                var newBook = new Book
                {
                    Id = GenerateRandomNumber(1000, 9999).ToString(),
                    Title = GenerateRandomString(15),
                    Description = GenerateRandomString(100),
                    PageCount = GenerateRandomString(15),
                    Excerpt = GenerateRandomString(50),
                    PublishDate = GenerateCurrentUtcDate()
                };
                var response = _bookRequest.PostNewInvalidBook(newBook);
                VerifyStatusCode(response, HttpStatusCode.BadRequest, "Unexpectedly succeeded in creating an invalid book");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                Console.WriteLine("Book with given ID have not been created.");
            }
        }

        [Test(Description = "Can not Create a new Book with invalid Publish Date")]
        public void CreateNewBookWithInvalidDate()
        {
            try
            {
                var newBook = new Book
                {
                    Id = GenerateRandomNumber(1000, 9999).ToString(),
                    Title = GenerateRandomString(15),
                    Description = GenerateRandomString(100),
                    PageCount = GenerateRandomNumber(100, 10000).ToString(),
                    Excerpt = GenerateRandomString(50),
                    PublishDate = GenerateRandomNumber(100, 10000).ToString()
                };
                var response = _bookRequest.PostNewInvalidBook(newBook);
                VerifyStatusCode(response, HttpStatusCode.BadRequest, "Unexpectedly succeeded in creating an invalid book");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                Console.WriteLine("Book with given ID have not been created.");
            }
        }

        [Test(Description = "Can Update existing Book by ID")]
        public void UpdateExistingBookById()
        {
            var getBookResponse = _bookRequest.GetBookById(updateBook.Id);
            var existingBook = DeserializeResponse<Book>(getBookResponse);
            Assert.IsNotNull(existingBook, "Book not found");

            var newBook = CreateRandomBook();
            var updateResponse = _bookRequest.UpdateBookById(existingBook.Id, newBook);
            var updatedBook = DeserializeResponse<Book>(updateResponse);

            VerifyBookData(newBook, updatedBook, "Updated book mismatch");
        }

        [Test(Description = "Can not Update (Create) Book with non-existing ID")]
        public void UpdateBookWithNonExistingId()
        {
            var bookId = GenerateRandomNumber(1000000, 5000000).ToString();
            var response = _bookRequest.GetBookById(bookId);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var newBook = CreateRandomBook();
                response = _bookRequest.UpdateBookById(bookId, newBook);
                var updatedBook = DeserializeResponse<Book>(response);
                VerifyBookData(newBook, updatedBook, "Updated book mismatch");
            }
            else
            {
                Console.WriteLine($"Unexpectedly found a book with ID {bookId}");
            }
        }

        [Test(Description = "Can Delete a Book by existing ID")]
        public void DeleteBookById()
        {
            var getBookResponse = _bookRequest.GetBookById(deleteBook.Id);
            var existingBook = DeserializeResponse<Book>(getBookResponse);
            Assert.IsNotNull(existingBook, "Book not found");

            var deleteResponse = _bookRequest.DeleteBookById(existingBook.Id);
            VerifyStatusCode(deleteResponse, HttpStatusCode.OK, $"Failed to delete book with ID {existingBook.Id}");

            var verifyResponse = _bookRequest.GetBookById(existingBook.Id);
            VerifyStatusCode(verifyResponse, HttpStatusCode.NotFound, $"Book with ID {existingBook.Id} was not deleted successfully");
        }

        [Test(Description = "Can not Delete a Book with non-existing ID")]
        public void DeleteBookWithNonExistingId()
        {
            var bookId = GenerateRandomNumber(1000000, 5000000).ToString();
            var getBookResponse = _bookRequest.GetBookById(bookId);
            VerifyStatusCode(getBookResponse, HttpStatusCode.NotFound, $"Found a book with ID {bookId}");

            var deleteResponse = _bookRequest.DeleteBookById(bookId);
            VerifyStatusCode(deleteResponse, HttpStatusCode.NotFound, $"Successfully deleted non-existing book with ID {bookId}");
        }

        [Test(Description = "Can not Delete a Book with alphabetical ID")]
        public void DeleteBookWithAlphabeticalId()
        {
            Assert.Throws<HttpRequestException>(() =>
            {
                var bookId = GenerateRandomString(3);
                var response = _bookRequest.DeleteBookById(bookId);
                VerifyStatusCode(response, HttpStatusCode.BadRequest, $"Request failed with status code: {response.StatusCode}");
                Console.WriteLine($"Book with ID: {bookId} have not been found.");
            });
        }

        [Test(Description = "Can not Delete a Book with negative ID")]
        public void DeleteBookByNegativeNumberId()
        {
            var bookId = GenerateRandomNumber(-1000, -1).ToString();
            Assert.Throws<HttpRequestException>(() =>
            {
                var response = _bookRequest.DeleteBookById(bookId);
                VerifyStatusCode(response, HttpStatusCode.BadRequest, $"Tried to delete book with negative ID: {bookId}");
            });
        }
    }
}
