using System.Net;
using Allure.NUnit;
using Newtonsoft.Json;
using OnlineBookstore.main.requests;
using OnlineBookstore.main.models;
using OnlineBookstore.main.utils;

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
            VerifyStatusCode(response, HttpStatusCode.OK, "Failed to retrieve books");
            
            var books = DeserializeResponse<List<Book>>(response);
            Console.WriteLine(JsonConvert.SerializeObject(books, Formatting.Indented));
            
            Assert.IsNotNull(books, "Books list is null");
            Assert.IsTrue(books.Count > 0, "Books list is empty");
        }

        [Test(Description = "Can Get Book by existing ID")]
        public void GetBookById()
        {
            var response = _bookRequest.GetBookById(existingBook.Id);
            VerifyStatusCode(response, HttpStatusCode.OK, "Failed to retrieve book");
            
            var book = DeserializeResponse<Book>(response);
            Assert.IsNotNull(book, "Book not found");
            
            VerifyData(existingBook.Id, book.Id, "Book ID does not match the expected value.");
            VerifyData(existingBook.Title, book.Title, "Book Title does not match the expected value.");
            VerifyData(existingBook.Description, book.Description, "Book Description does not match the expected value.");
            VerifyData(existingBook.PageCount, book.PageCount, "Book PageCount does not match the expected value.");
            VerifyData(existingBook.Excerpt, book.Excerpt, "Book Excerpt does not match the expected value.");
            VerifyData(existingBook.PublishDate, book.PublishDate, "Book PublishDate does not match the expected value.");
        }

        [Test(Description = "Can Get Book with non-existing positive ID")]
        public void GetBookByNonExistingId()
        {
            var bookId = GenerateRandomNumber(1000000, 5000000).ToString();
            var response = _bookRequest.GetBookById(bookId);
            
            VerifyStatusCode(response, HttpStatusCode.NotFound, "Book found unexpectedly");
            Console.WriteLine($"Book with ID {bookId} not found");
        }
        
        [Test(Description = "Can not Get Book with alphabetical ID")]
        public void GetBookByAlphabeticalId()
        {
            Assert.Throws<HttpRequestException>(() =>
            {
                var response = _bookRequest.GetBookById(GenerateRandomString(3));
                VerifyStatusCode(response, HttpStatusCode.BadRequest, $"Request failed with status code: {response.StatusCode}");
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
            var newBook = new Book
            {
                Id = GenerateRandomNumber(1000, 3999).ToString(),
                Title = GenerateRandomString(15),
                Description = GenerateRandomString(100),
                PageCount = GenerateRandomNumber(100, 10000).ToString(),
                Excerpt = GenerateRandomString(50),
                PublishDate = GenerateCurrentUtcDate()
            };
            
            var response = _bookRequest.PostNewBook(newBook);
            VerifyStatusCode(response, HttpStatusCode.OK, "Failed to create a new book");

            var createdBook = DeserializeResponse<Book>(response);
            Assert.IsNotNull(createdBook, "Book not found");
            
            VerifyData(existingBook.Id, createdBook.Id, "Book ID does not match the Created Book ID.");
            VerifyData(existingBook.Title, createdBook.Title, "Book Title does not match the Created Book Title.");
            VerifyData(existingBook.Description, createdBook.Description, "Book Description does not match the Created Book Descritpion.");
            VerifyData(existingBook.PageCount, createdBook.PageCount, "Book PageCount does not match the Created Book PageCount.");
            VerifyData(existingBook.Excerpt, createdBook.Excerpt, "Book Excerpt does not match the Created Book Excerpt.");
            VerifyData(existingBook.PublishDate, createdBook.PublishDate, "Book PublishDate does not match the Created Book PublishDate.");
            
            var verifyBookById = _bookRequest.GetBookById(newBook.Id);
            VerifyStatusCode(verifyBookById, HttpStatusCode.OK, "Created new book is not successfully saved");

            var book = DeserializeResponse<Book>(verifyBookById);
            Assert.IsNotNull(book, "Book not found");
            
            VerifyData(createdBook.Id, book.Id, "Created Book ID does not match the Expected Book ID.");
            VerifyData(createdBook.Title, book.Title, "Created Book Title does not match the Expected Book Title.");
            VerifyData(createdBook.Description, book.Description, "Created Book Description does not match the Expected Book Descritpion.");
            VerifyData(createdBook.PageCount, book.PageCount, "Created Book PageCount does not match the Expected Book PageCount.");
            VerifyData(createdBook.Excerpt, book.Excerpt, "Created Book Excerpt does not match the Expected Book Excerpt.");
            VerifyData(createdBook.PublishDate, book.PublishDate, "Created Book PublishDate does not match the Expected Book PublishDate.");
        }

        [Test(Description = "Can not Create a new Book with invalid ID")]
        public void CreateNewBookWithInvalidId()
        {
            try
            {
                var newBook = new Book
                {
                    Id = GenerateRandomNumber(1000, 3999).ToString(),
                    Title = GenerateRandomString(15),
                    Description = GenerateRandomString(100),
                    PageCount = GenerateRandomNumber(100, 10000).ToString(),
                    Excerpt = GenerateRandomString(50),
                    //PublishDate = 
                    
                    // Id = GenerateRandomString(15),
                    // IdBook = GenerateRandomString(15),
                    // FirstName = GenerateRandomString(15),
                    // LastName = GenerateRandomString(15)
                };
                
                var response = _bookRequest.PostNewInvalidBook(newBook);
                VerifyStatusCode(response, HttpStatusCode.BadRequest, "Unexpectedly succeeded in creating an invalid book");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
            }
            
            // Additional verification to ensure the invalid book does not exist
        }

        //TODO: Refactor this test
        [Test(Description = "Can Update existing Book by ID")] 
        public void UpdateExistingBookById()
        {
            var getBookResponse = _bookRequest.GetBookById(updateBook.Id);
            VerifyStatusCode(getBookResponse, HttpStatusCode.OK, "Failed to get a book");
            var existingBook = DeserializeResponse<Book>(getBookResponse);
            Assert.IsNotNull(existingBook, "Book not found");
            
            var newBook = new Book
            {
                Id = GenerateRandomNumber(1000, 3999).ToString(),
                Title = GenerateRandomString(15),
                Description = GenerateRandomString(100),
                PageCount = GenerateRandomNumber(100, 10000).ToString(),
                Excerpt = GenerateRandomString(50),
                //PublishDate = 
                
                // Id = existingBook.Id,
                // IdBook = GenerateRandomNumber(1, 100).ToString(),
                // FirstName = GenerateRandomString(15),
                // LastName = GenerateRandomString(15)
            };
            
            var response = _bookRequest.UpdateBookById(newBook.Id, newBook);
            VerifyStatusCode(response, HttpStatusCode.OK, "Failed to update a book");
            
            var updatedBook = DeserializeResponse<Book>(response);
            Assert.IsNotNull(updatedBook, "Book not found");
            
            Console.WriteLine(JsonConvert.SerializeObject(updatedBook, Formatting.Indented));
            
            VerifyData(newBook.Id, updatedBook.Id, "New Book ID does not match the Updated Book ID.");
            VerifyData(newBook.Title, updatedBook.Title, "New Book Title does not match the Updated Book Title.");
            VerifyData(newBook.Description, updatedBook.Description, "New Book Description does not match the Updated Book Descritpion.");
            VerifyData(newBook.PageCount, updatedBook.PageCount, "New Book PageCount does not match the Updated Book PageCount.");
            VerifyData(newBook.Excerpt, updatedBook.Excerpt, "New Book Excerpt does not match the Updated Book Excerpt.");
            VerifyData(newBook.PublishDate, updatedBook.PublishDate, "New ook PublishDate does not match the Updated Book PublishDate.");
            
            var verifyBookById = _bookRequest.GetBookById(newBook.Id);
            VerifyStatusCode(verifyBookById, HttpStatusCode.OK, "Created new book is not successfully saved");

            var book = DeserializeResponse<Book>(verifyBookById);
            Assert.IsNotNull(book, "Book not found");
            
            VerifyData(updatedBook.Id, book.Id, "Updated Book ID does not match the Expected Book ID.");
            VerifyData(updatedBook.Title, book.Title, "Updated Book Title does not match the Expected Book Title.");
            VerifyData(updatedBook.Description, book.Description, "Updated Book Description does not match the Expected Book Descritpion.");
            VerifyData(updatedBook.PageCount, book.PageCount, "Updated Book PageCount does not match the Expected Book PageCount.");
            VerifyData(updatedBook.Excerpt, book.Excerpt, "Updated Book Excerpt does not match the Expected Book Excerpt.");
            VerifyData(updatedBook.PublishDate, book.PublishDate, "Updated Book PublishDate does not match the Expected Book PublishDate.");
        }

        [Test(Description = "Can not Update Book with non-existing ID")]
        public void UpdateBookWithNonExistingId()
        {
            var bookId = GenerateRandomNumber(1000000, 5000000).ToString();
            var response = _bookRequest.GetBookById(bookId);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var newBook = new Book
                {
                    Id = GenerateRandomNumber(1000, 3999).ToString(),
                    Title = GenerateRandomString(15),
                    Description = GenerateRandomString(100),
                    PageCount = GenerateRandomNumber(100, 10000).ToString(),
                    Excerpt = GenerateRandomString(50),
                    //PublishDate = 
                    
                    
                    // Id = bookId,
                    // IdBook = GenerateRandomNumber(1, 100).ToString(),
                    // FirstName = GenerateRandomString(15),
                    // LastName = GenerateRandomString(15)
                };
                
                response = _bookRequest.UpdateBookById(bookId, newBook);
                VerifyStatusCode(response, HttpStatusCode.OK, "Failed to create a new book with non-existing ID");
                
                var updatedBook = DeserializeResponse<Book>(response);
                Assert.IsNotNull(updatedBook, "Book not found");
                Console.WriteLine(JsonConvert.SerializeObject(updatedBook, Formatting.Indented));
                
                VerifyData(newBook.Id, updatedBook.Id, "New Book ID does not match the Updated Book ID.");
                VerifyData(newBook.Title, updatedBook.Title, "New Book Title does not match the Updated Book Title.");
                VerifyData(newBook.Description, updatedBook.Description, "New Book Description does not match the Updated Book Descritpion.");
                VerifyData(newBook.PageCount, updatedBook.PageCount, "New Book PageCount does not match the Updated Book PageCount.");
                VerifyData(newBook.Excerpt, updatedBook.Excerpt, "New Book Excerpt does not match the Updated Book Excerpt.");
                VerifyData(newBook.PublishDate, updatedBook.PublishDate, "New ook PublishDate does not match the Updated Book PublishDate.");
            
                var verifyBookById = _bookRequest.GetBookById(newBook.Id);
                VerifyStatusCode(verifyBookById, HttpStatusCode.OK, "Created new book is not successfully saved");

                var book = DeserializeResponse<Book>(verifyBookById);
                Assert.IsNotNull(book, "Book not found");
                
                VerifyData(updatedBook.Id, book.Id, "Updated Book ID does not match the Expected Book ID.");
                VerifyData(updatedBook.Title, book.Title, "Updated Book Title does not match the Expected Book Title.");
                VerifyData(updatedBook.Description, book.Description, "Updated Book Description does not match the Expected Book Descritpion.");
                VerifyData(updatedBook.PageCount, book.PageCount, "Updated Book PageCount does not match the Expected Book PageCount.");
                VerifyData(updatedBook.Excerpt, book.Excerpt, "Updated Book Excerpt does not match the Expected Book Excerpt.");
                VerifyData(updatedBook.PublishDate, book.PublishDate, "Updated Book PublishDate does not match the Expected Book PublishDate.");
                
            } else if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine($"Found a book with ID {bookId}");
            }
            else
            {
                Console.WriteLine($"Request failed with status code {response.StatusCode}");
            }
        }

        [Test(Description = "Can Delete a Book by existing ID")]
        public void DeleteBookById()
        {
            var getBookResponse = _bookRequest.GetBookById(deleteBook.Id);
            VerifyStatusCode(getBookResponse, HttpStatusCode.OK, "Failed to get a book");
            
            var existingBook = DeserializeResponse<Book>(getBookResponse);
            Assert.IsNotNull(existingBook, "Book not found");
            
            var deleteResponse = _bookRequest.DeleteBookById(existingBook.Id);
            VerifyStatusCode(deleteResponse, HttpStatusCode.OK, "Failed to delete book");

            var verifyResponse = _bookRequest.GetBookById(existingBook.Id);
            VerifyStatusCode(verifyResponse, HttpStatusCode.NotFound, "Book was not deleted successfully");
        }

        [Test(Description = "Can not Delete a Book with non-existing ID")]
        public void DeleteBookWithNonExistingId()
        {
            var bookId = GenerateRandomNumber(1000000, 5000000).ToString();
            
            var getBookResponse = _bookRequest.GetBookById(bookId);
            VerifyStatusCode(getBookResponse, HttpStatusCode.NotFound, $"Found a book with ID {bookId}");
            
            var deleteResponse = _bookRequest.DeleteBookById(bookId);
            VerifyStatusCode(deleteResponse, HttpStatusCode.NotFound, "Successfully deleted non-existing book");
        }

        [Test(Description = "Can not Delete a Book with alphabetical ID")]
        public void DeleteBookWithAlphabeticalId()
        {
            Assert.Throws<HttpRequestException>(() =>
            {
                var response = _bookRequest.DeleteBookById(GenerateRandomString(3));
                VerifyStatusCode(response, HttpStatusCode.BadRequest, $"Request failed with status code: {response.StatusCode}");
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
