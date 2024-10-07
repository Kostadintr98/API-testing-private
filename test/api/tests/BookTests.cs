using System.Net;
using Allure.NUnit;
using OnlineBookstore.main.requests;
using OnlineBookstore.main.models;
using OnlineBookstore.main.utils;
using OnlineBookstore.test.data_prоvider;
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

        [Test, TestCaseSource(typeof(BooksData), nameof(BooksData.GetBookWithInvalidData))]
        public void GetBookWithInvalidId(string bookId, HttpStatusCode expectedStatusCode, string errorMessage)
        {
            try
            {
                var response = _bookRequest.GetBookById(bookId);
                VerifyStatusCode(response, expectedStatusCode, $"Unexpectedly succeeded in fetching a book with invalid ID: {errorMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                Console.WriteLine($"Fetching book with invalid ID failed: {errorMessage}");
            }
        }

        [Test(Description = "Can Create a new Book")]
        public void CreateNewBook()
        {
            var newBook = CreateRandomBook();
            var response = _bookRequest.PostNewBook(newBook);
            var createdBook = DeserializeResponse<Book>(response);
            VerifyBookData(newBook, createdBook, "Created book mismatch");
        }
        
        [Test, TestCaseSource(typeof(BooksData), nameof(BooksData.CreateBookWithInvalidData))]
        public void CreateNewBookWithInvalidData(Book newBook, string errorMessage)
        {
            try
            {
                var response = _bookRequest.PostNewInvalidBook(newBook);
                VerifyStatusCode(response, HttpStatusCode.BadRequest, $"Unexpectedly succeeded in creating a book with invalid data: {errorMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                Console.WriteLine($"Book with invalid data '{errorMessage}' was not created.");
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

        [Test, TestCaseSource(typeof(BooksData), nameof(BooksData.DeleteBookWithInvalidData))]
        public void DeleteBookWithInvalidId(string bookId, HttpStatusCode expectedStatusCode, string errorMessage)
        {
            try
            {
                var deleteResponse = _bookRequest.DeleteBookById(bookId);
                VerifyStatusCode(deleteResponse, expectedStatusCode, $"Unexpectedly succeeded in deleting a book with invalid ID: {errorMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                Console.WriteLine($"Deletion of book with invalid ID failed: {errorMessage}");
            }
        }
    }
}
