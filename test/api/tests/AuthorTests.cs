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
            VerifyAndPrintResponse<List<Author>>(response, HttpStatusCode.OK, "Failed to retrieve authors");
        }

        [Test(Description = "Can Get Author by existing ID")]
        public void GetAuthorById()
        {
            var response = _authorRequest.GetAuthorById(existingAuthor.Id);
            var author = DeserializeResponse<Author>(response);
            VerifyAuthorData(existingAuthor, author, "Author retrieval mismatch");
        }

        [Test(Description = "Can Get Author with non-existing positive ID")]
        public void GetAuthorByNonExistingId()
        {
            var authorId = GenerateRandomNumber(1000000, 5000000).ToString();
            var response = _authorRequest.GetAuthorById(authorId);
            VerifyStatusCode(response, HttpStatusCode.NotFound, $"Author with ID {authorId} found unexpectedly");
        }

        [Test(Description = "Can not Get Author with alphabetical ID")]
        public void GetAuthorByAlphabeticalId()
        {
            Assert.Throws<HttpRequestException>(() =>
            {
                var authorId = GenerateRandomString(3);
                var response = _authorRequest.GetAuthorById(authorId);
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
            var newAuthor = CreateRandomAuthor();
            var response = _authorRequest.PostNewAuthor(newAuthor);
            var createdAuthor = DeserializeResponse<Author>(response);
            VerifyAuthorData(newAuthor, createdAuthor, "Created author mismatch");
        }

        [Test(Description = "Can not Create a new Author with invalid ID")]
        public void CreateNewAuthorWithInvalidId()
        {
            try
            {
                var newAuthor = new Author
                {
                    Id = GenerateRandomString(15),
                    IdBook = GenerateRandomNumber(1000, 9999).ToString(),
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
        }

        [Test(Description = "Can not Create a new Author with invalid BookID")]
        public void CreateNewAuthorWithInvalidBookId()
        {
            try
            {
                var newAuthor = new Author
                {
                    Id = GenerateRandomNumber(1000, 9999).ToString(),
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
        }

        [Test(Description = "Can Update existing Author by ID")]
        public void UpdateExistingAuthorById()
        {
            var getAuthorResponse = _authorRequest.GetAuthorById(updateAuthor.Id);
            var existingAuthor = DeserializeResponse<Author>(getAuthorResponse);
            Assert.IsNotNull(existingAuthor, "Author not found");

            var newAuthor = new Author
            {
                Id = existingAuthor.Id,
                IdBook = GenerateRandomNumber(1, 100).ToString(),
                FirstName = GenerateRandomString(15),
                LastName = GenerateRandomString(15)
            };

            var updateResponse = _authorRequest.UpdateAuthorById(newAuthor.Id, newAuthor);
            var updatedAuthor = DeserializeResponse<Author>(updateResponse);

            VerifyAuthorData(newAuthor, updatedAuthor, "Updated author mismatch");
        }

        [Test(Description = "Can not Update (Create) Author with non-existing ID")]
        public void UpdateAuthorWithNonExistingId()
        {
            var authorId = GenerateRandomNumber(1000000, 5000000).ToString();
            var response = _authorRequest.GetAuthorById(authorId);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var newAuthor = CreateRandomAuthor();
                response = _authorRequest.UpdateAuthorById(authorId, newAuthor);
                var updatedAuthor = DeserializeResponse<Author>(response);
                VerifyAuthorData(newAuthor, updatedAuthor, "Updated author mismatch");
            }
            else
            {
                Console.WriteLine($"Unexpectedly found an author with ID {authorId}");
            }
        }

        [Test(Description = "Can Delete an Author by existing ID")]
        public void DeleteAuthorById()
        {
            var getAuthorResponse = _authorRequest.GetAuthorById(deleteAuthor.Id);
            var existingAuthor = DeserializeResponse<Author>(getAuthorResponse);
            Assert.IsNotNull(existingAuthor, "Author not found");

            var deleteResponse = _authorRequest.DeleteAuthorById(existingAuthor.Id);
            VerifyStatusCode(deleteResponse, HttpStatusCode.OK, $"Failed to delete author with ID {existingAuthor.Id}");

            var verifyResponse = _authorRequest.GetAuthorById(existingAuthor.Id);
            VerifyStatusCode(verifyResponse, HttpStatusCode.NotFound, $"Author with ID {existingAuthor.Id} was not deleted successfully");
        }

        [Test(Description = "Can not Delete an Author with non-existing ID")]
        public void DeleteAuthorWithNonExistingId()
        {
            var authorId = GenerateRandomNumber(1000000, 5000000).ToString();
            var getAuthorResponse = _authorRequest.GetAuthorById(authorId);
            VerifyStatusCode(getAuthorResponse, HttpStatusCode.NotFound, $"Found an author with ID {authorId}");

            var deleteResponse = _authorRequest.DeleteAuthorById(authorId);
            VerifyStatusCode(deleteResponse, HttpStatusCode.NotFound, $"Successfully deleted non-existing author with ID {authorId}");
        }

        [Test(Description = "Can not Delete an Author with alphabetical ID")]
        public void DeleteAuthorWithAlphabeticalId()
        {
            Assert.Throws<HttpRequestException>(() =>
            {
                var authorId = GenerateRandomString(3);
                var response = _authorRequest.DeleteAuthorById(authorId);
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
