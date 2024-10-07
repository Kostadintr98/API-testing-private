using System.Net;
using Allure.NUnit;
using OnlineBookstore.main.requests;
using OnlineBookstore.main.models;
using OnlineBookstore.main.utils;
using OnlineBookstore.test.data_prоvider;

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

        [Test, TestCaseSource(typeof(AuthorData), nameof(AuthorData.GetAuthorWithInvalidData))]
        public void GetAuthorWithInvalidId(string authorId, HttpStatusCode expectedStatusCode, string errorMessage)
        {
            try
            {
                var response = _authorRequest.GetAuthorById(authorId);
                VerifyStatusCode(response, expectedStatusCode, $"Unexpectedly succeeded in fetching an author with invalid ID: {errorMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                Console.WriteLine($"Fetching author with invalid ID failed: {errorMessage}");
            }
        }

        [Test(Description = "Can Create a new Author")]
        public void CreateNewAuthor()
        {
            var newAuthor = CreateRandomAuthor();
            var response = _authorRequest.PostNewAuthor(newAuthor);
            var createdAuthor = DeserializeResponse<Author>(response);
            VerifyAuthorData(newAuthor, createdAuthor, "Created author mismatch");
        }

        [Test, TestCaseSource(typeof(AuthorData), nameof(AuthorData.CreateAuthorWithInvalidData))]
        public void CreateNewAuthorWithInvalidData(Author newAuthor, string errorMessage)
        {
            try
            {
                var response = _authorRequest.PostNewInvalidAuthor(newAuthor);
                VerifyStatusCode(response, HttpStatusCode.BadRequest, $"Unexpectedly succeeded in creating an author with invalid data: {errorMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                Console.WriteLine($"Author with invalid data '{errorMessage}' was not created.");
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
        
        [Test, TestCaseSource(typeof(AuthorData), nameof(AuthorData.DeleteAuthorWithInvalidData))]
        public void DeleteAuthorWithInvalidId(string authorId, HttpStatusCode expectedStatusCode, string errorMessage)
        {
            try
            {
                var deleteResponse = _authorRequest.DeleteAuthorById(authorId);
                VerifyStatusCode(deleteResponse, expectedStatusCode, $"Unexpectedly succeeded in deleting an author with invalid ID: {errorMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                Console.WriteLine($"Deletion of author with invalid ID failed: {errorMessage}");
            }
        }
    }
}
