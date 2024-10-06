using System.Net;
using Allure.NUnit;
using Newtonsoft.Json;
using OnlineBookstore.main.requests;
using OnlineBookstore.main.models;
using OnlineBookstore.main.utils;

namespace OnlineBookstore.test.api.tests
{
    [AllureNUnit]
    public class AuthorTests : AuthorHelper
    {
        private AuthorRequests _authorRequest;

        [SetUp]
        public void Setup()
        {
            _authorRequest = new AuthorRequests();
        }

        [Test(Description = "Can Get all Authors")]
        public void GetAllAuthors()
        {
            var response = _authorRequest.GetAllAuthors();
            VerifyStatusCode(response, HttpStatusCode.OK, "Failed to retrieve authors");
            
            var authors = DeserializeResponse<List<Author>>(response);
            Console.WriteLine(JsonConvert.SerializeObject(authors, Formatting.Indented));
            
            Assert.IsNotNull(authors, "Authors list is null");
            Assert.IsTrue(authors.Count > 0, "Authors list is empty");
        }

        [Test(Description = "Can Get Author by existing ID")]
        public void GetAuthorById()
        {
            var response = _authorRequest.GetAuthorById(existingAuthor.Id);
            VerifyStatusCode(response, HttpStatusCode.OK, "Failed to retrieve author");
            
            var author = DeserializeResponse<Author>(response);
            Assert.IsNotNull(author, "Author not found");
            
            VerifyData(existingAuthor.Id, author.Id, "Author ID does not match the expected value.");
            VerifyData(existingAuthor.IdBook, author.IdBook, "Author BookID does not match the expected value.");
            VerifyData(existingAuthor.FirstName, author.FirstName, "Author First Name does not match the expected value.");
            VerifyData(existingAuthor.LastName, author.LastName, "Author Last Name does not match the expected value.");
        }

        [Test(Description = "Can Get Author with non-existing positive ID")]
        public void GetAuthorByNonExistingId()
        {
            var authorId = GenerateRandomNumber(1000000, 5000000).ToString();
            var response = _authorRequest.GetAuthorById(authorId);
            
            VerifyStatusCode(response, HttpStatusCode.NotFound, "Author found unexpectedly");
            Console.WriteLine($"Author with ID {authorId} not found");
        }
        
        [Test(Description = "Can not Get Author with alphabetical ID")]
        public void GetAuthorByAlphabeticalId()
        {
            Assert.Throws<HttpRequestException>(() =>
            {
                var response = _authorRequest.GetAuthorById(GenerateRandomString(3));
                VerifyStatusCode(response, HttpStatusCode.BadRequest, $"Request failed with status code: {response.StatusCode}");
            });
        }
        
        [Test(Description = "Can not Get Author with negative number ID")]
        public void GetAuthorByNegativeNumberId()
        {
            var authorId = GenerateRandomNumber(-1000, -1).ToString();
            var response = _authorRequest.GetAuthorById(authorId);
            VerifyStatusCode(response, HttpStatusCode.NotFound, $"Request failed with status code: {response.StatusCode}");
        }

        [Test(Description = "Can Create a new Author")]
        public void CreateNewAuthor()
        {
            var newAuthor = new Author
            {
                Id = GenerateRandomNumber(1000, 3999).ToString(),
                IdBook = GenerateRandomNumber(4000, 7999).ToString(),
                FirstName = GenerateRandomString(15),
                LastName = GenerateRandomString(15)
            };
            
            var response = _authorRequest.PostNewAuthor(newAuthor);
            VerifyStatusCode(response, HttpStatusCode.OK, "Failed to create a new author");

            var createdAuthor = DeserializeResponse<Author>(response);
            Assert.IsNotNull(createdAuthor, "Author not found");
            
            VerifyData(newAuthor.Id, createdAuthor.Id, "New Author ID does not match the Created Author ID.");
            VerifyData(newAuthor.IdBook, createdAuthor.IdBook, "New Author BookID does not match the Created Author BookID.");
            VerifyData(newAuthor.FirstName, createdAuthor.FirstName, "New Author First Name does not match the Created Author First Name.");
            VerifyData(newAuthor.LastName, createdAuthor.LastName, "New Author Last Name does not match the Created Author Last Name.");

            var verifyAuthorById = _authorRequest.GetAuthorById(newAuthor.Id);
            VerifyStatusCode(verifyAuthorById, HttpStatusCode.OK, "Created new author is not successfully saved");

            var author = DeserializeResponse<Author>(verifyAuthorById);
            Assert.IsNotNull(author, "Author not found");
            
            VerifyData(createdAuthor.Id, author.Id, "Created Author ID does not match the Expected Author ID.");
            VerifyData(createdAuthor.IdBook, author.IdBook, "Created Author BookID does not match the Expected Author BookID.");
            VerifyData(createdAuthor.FirstName, author.FirstName, "Created Author First Name does not match the Expected Author First Name.");
            VerifyData(createdAuthor.LastName, author.LastName, "Created Author Last Name does not match the Expected Author Last Name.");
        }

        [Test(Description = "Can not Create a new Author with invalid ID")]
        public void CreateNewAuthorWithInvalidId()
        {
            try
            {
                var newAuthor = new Author
                {
                    Id = GenerateRandomString(15),
                    //IdBook = GenerateRandomString(15),
                    FirstName = GenerateRandomString(15),
                    LastName = GenerateRandomString(15)
                };
                
                var response = _authorRequest.PostNewInvalidAuthor(newAuthor);
                VerifyStatusCode(response, HttpStatusCode.BadRequest, "Unexpectedly succeeded in creating an invalid author");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
            }
            
            // Additional verification to ensure the invalid author does not exist
        }
        
        [Test(Description = "Can not Create a new Author with invalid BookID")]
        public void CreateNewAuthorWithInvalidBookId()
        {
            try
            {
                var newAuthor = new Author
                {
                    //Id = GenerateRandomString(15),
                    IdBook = GenerateRandomString(15),
                    FirstName = GenerateRandomString(15),
                    LastName = GenerateRandomString(15)
                };
                
                var response = _authorRequest.PostNewInvalidAuthor(newAuthor);
                VerifyStatusCode(response, HttpStatusCode.BadRequest, "Unexpectedly succeeded in creating an invalid author");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
            }
            
            // Additional verification to ensure the invalid author does not exist
        }
        
        [Test(Description = "Can Update existing Author by ID")] 
        public void UpdateExistingAuthorById()
        {
            var getAuthorResponse = _authorRequest.GetAuthorById(updateAuthor.Id);
            VerifyStatusCode(getAuthorResponse, HttpStatusCode.OK, "Failed to get an author");
            var existingAuthor = DeserializeResponse<Author>(getAuthorResponse);
            Assert.IsNotNull(existingAuthor, "Author not found");
            
            var newAuthor = new Author
            {
                Id = existingAuthor.Id,
                IdBook = GenerateRandomNumber(1, 100).ToString(),
                FirstName = GenerateRandomString(15),
                LastName = GenerateRandomString(15)
            };
            
            var response = _authorRequest.UpdateAuthorById(newAuthor.Id, newAuthor);
            VerifyStatusCode(response, HttpStatusCode.OK, "Failed to update an author");
            
            var updatedAuthor = DeserializeResponse<Author>(response);
            Assert.IsNotNull(updatedAuthor, "Author not found");
            
            Console.WriteLine(JsonConvert.SerializeObject(updatedAuthor, Formatting.Indented));
            
            VerifyData(newAuthor.Id, updatedAuthor.Id, "New Author ID does not match the Updated Author ID.");
            VerifyData(newAuthor.IdBook, updatedAuthor.IdBook, "New Author BookID does not match the Updated Author BookID.");
            VerifyData(newAuthor.FirstName, updatedAuthor.FirstName, "New Author First Name does not match the Updated Author First Name.");
            VerifyData(newAuthor.LastName, updatedAuthor.LastName, "New Author Last Name does not match the Updated Author Last Name.");
            
            var verifyAuthorById = _authorRequest.GetAuthorById(newAuthor.Id);
            VerifyStatusCode(verifyAuthorById, HttpStatusCode.OK, "Created new author is not successfully saved");

            var author = DeserializeResponse<Author>(verifyAuthorById);
            Assert.IsNotNull(author, "Author not found");
            
            VerifyData(updatedAuthor.Id, author.Id, "Updated Author ID does not match the Expected Author ID.");
            VerifyData(updatedAuthor.IdBook, author.IdBook, "Updated Author BookID does not match the Expected Author BookID.");
            VerifyData(updatedAuthor.FirstName, author.FirstName, "Updated Author First Name does not match the Expected Author First Name.");
            VerifyData(updatedAuthor.LastName, author.LastName, "Updated Author Last Name does not match the Expected Author Last Name.");
        }

        [Test(Description = "Can not Update Author with non-existing ID")]
        public void UpdateAuthorWithNonExistingId()
        {
            var authorId = GenerateRandomNumber(1000000, 5000000).ToString();
            var response = _authorRequest.GetAuthorById(authorId);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var newAuthor = new Author
                {
                    Id = authorId,
                    IdBook = GenerateRandomNumber(1, 100).ToString(),
                    FirstName = GenerateRandomString(15),
                    LastName = GenerateRandomString(15)
                };
                
                response = _authorRequest.UpdateAuthorById(authorId, newAuthor);
                VerifyStatusCode(response, HttpStatusCode.OK, "Failed to create a new author with non-existing ID");
                
                var updatedAuthor = DeserializeResponse<Author>(response);
                Assert.IsNotNull(updatedAuthor, "Author not found");
                Console.WriteLine(JsonConvert.SerializeObject(updatedAuthor, Formatting.Indented));
                
                VerifyData(newAuthor.Id, updatedAuthor.Id, "New Author ID does not match the Updated Author ID.");
                VerifyData(newAuthor.IdBook, updatedAuthor.IdBook, "New Author BookID does not match the Updated Author BookID.");
                VerifyData(newAuthor.FirstName, updatedAuthor.FirstName, "New Author First Name does not match the Updated Author First Name.");
                VerifyData(newAuthor.LastName, updatedAuthor.LastName, "New Author Last Name does not match the Updated Author Last Name.");
            
                var verifyAuthorById = _authorRequest.GetAuthorById(newAuthor.Id);
                VerifyStatusCode(verifyAuthorById, HttpStatusCode.OK, "Created new author is not successfully saved");

                var author = DeserializeResponse<Author>(verifyAuthorById);
                Assert.IsNotNull(author, "Author not found");
                
                VerifyData(updatedAuthor.Id, author.Id, "Updated Author ID does not match the Expected Author ID.");
                VerifyData(updatedAuthor.IdBook, author.IdBook, "Updated Author BookID does not match the Expected Author BookID.");
                VerifyData(updatedAuthor.FirstName, author.FirstName, "Updated Author First Name does not match the Expected Author First Name.");
                VerifyData(updatedAuthor.LastName, author.LastName, "Updated Author Last Name does not match the Expected Author Last Name.");
                
            } else if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine($"Found an author with ID {authorId}");
            }
            else
            {
                Console.WriteLine($"Request failed with status code {response.StatusCode}");
            }
        }

        [Test(Description = "Can Delete an Author by exising ID")]
        public void DeleteAuthorById()
        {
            var getAuthorResponse = _authorRequest.GetAuthorById(deleteAuthor.Id);
            VerifyStatusCode(getAuthorResponse, HttpStatusCode.OK, "Failed to get an author");
            
            var existingAuthor = DeserializeResponse<Author>(getAuthorResponse);
            Assert.IsNotNull(existingAuthor, "Author not found");
            
            var deleteResponse = _authorRequest.DeleteAuthorById(existingAuthor.Id);
            VerifyStatusCode(deleteResponse, HttpStatusCode.OK, "Failed to delete author");

            var verifyResponse = _authorRequest.GetAuthorById(existingAuthor.Id);
            VerifyStatusCode(verifyResponse, HttpStatusCode.NotFound, "Author was not deleted successfully");
        }

        [Test(Description = "Can not Delete an Author with non-existing ID")]
        public void DeleteAuthorWithNonExistingId()
        {
            var authorId = GenerateRandomNumber(1000000, 5000000).ToString();
            
            var getAuthorResponse = _authorRequest.GetAuthorById(authorId);
            VerifyStatusCode(getAuthorResponse, HttpStatusCode.NotFound, $"Found an author with ID {authorId}");
            
            var deleteResponse = _authorRequest.DeleteAuthorById(authorId);
            VerifyStatusCode(deleteResponse, HttpStatusCode.NotFound, "Successfully deleted non-existing author");
        }

        [Test(Description = "Can not Delete an Author with aplhabetical ID")]
        public void DeleteAuthorWithAlphabeticalId()
        {
            Assert.Throws<HttpRequestException>(() =>
            {
                var response = _authorRequest.DeleteAuthorById(GenerateRandomString(3));
                VerifyStatusCode(response, HttpStatusCode.BadRequest, $"Request failed with status code: {response.StatusCode}");
            });
        }
        
        [Test(Description = "Can not Delete an Author with negative ID")]
        public void DeleteAuthorByNegativeNumberId()
        {
            var authorId = GenerateRandomNumber(-1000, -1).ToString();
            Assert.Throws<HttpRequestException>(() =>
            {
                var response = _authorRequest.DeleteAuthorById(authorId);
                VerifyStatusCode(response, HttpStatusCode.BadRequest, $"Tried to delete author with negative ID: {authorId}");
            });
        }
    }
}
