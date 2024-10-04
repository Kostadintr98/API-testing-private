using System.Net;
using Newtonsoft.Json;
using OnlineBookstore.main.requests;
using OnlineBookstore.main.models;

namespace OnlineBookstore.test.api.tests
{
    public class AuthorTests
    {
        private AuthorRequests _authorRequest;

        [SetUp]
        public void Setup()
        {
            _authorRequest = new AuthorRequests();
        }

        [Test]
        public void GetAllAuthors()
        {
            var response = _authorRequest.GetAllAuthors();
            
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Failed to retrieve authors");
            
            var authors = BaseRequests.DeserializeResponse<List<Author>>(response);
            
            Console.WriteLine(JsonConvert.SerializeObject(authors, Formatting.Indented));
            
            Assert.IsNotNull(authors, "Authors list is null");
            Assert.IsTrue(authors.Count > 0, "Authors list is empty");
        }

        [Test]
        public void GetAuthorById()
        {
            var response = _authorRequest.GetAuthorById(_authorRequest._config["Author:Id"]);
            
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Failed to retrieve author");
            
            var author = BaseRequests.DeserializeResponse<Author>(response);
            
            Assert.That(author, Is.Not.Null, "Author not found");
            
            int expectedAuthorId = int.Parse(_authorRequest._config["ExpectedAuthor:Id"]);
            int expectedBookId = int.Parse(_authorRequest._config["ExpectedAuthor:BookId"]);
            var expectedFirstName = _authorRequest._config["ExpectedAuthor:FirstName"];
            var expectedLastName = _authorRequest._config["ExpectedAuthor:LastName"];
            
            Console.WriteLine(
                $"Author Id: {author.Id}, Book Referance: {author.IdBook}, Name: {author.FirstName} {author.LastName}");
            
            Assert.AreEqual(expectedAuthorId, author.Id, "Author ID does not match the expected value.");
            Assert.AreEqual(expectedBookId, author.IdBook, "Author's book ID does not match the expected value.");
            Assert.AreEqual(expectedFirstName, author.FirstName, "Author's first name does not match the expected value.");
            Assert.AreEqual(expectedLastName, author.LastName, "Author's last name does not match the expected value.");
        }

        [Test]
        public void GetAuthorByNotExistingId()
        {
            var response = _authorRequest.GetAuthorById(_authorRequest._config["NotExistingAuthor:Id"]);
            
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.NotFound, "Author found unexpectedly");

            Console.WriteLine($"Author with ID {_authorRequest._config["NotExistingAuthor:Id"]} not found");
        }

        [Test] 
        public void GetAuthorByInvalidId()
        {
            try
            {
                var response = _authorRequest.GetAuthorById(_authorRequest._config["InvalidAuthor:Id"]);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    Console.WriteLine("BadRequest: Check the request parameters.");
                }
                else if (!response.IsSuccessful)
                {
                    Console.WriteLine($"Request failed with status code {response.StatusCode}");
                }
                else
                {
                    Console.WriteLine(response.Content);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

        [Test]
        public void CreateNewAuthor()
        {
            var response = _authorRequest.PostNewAuthor();
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Failed to create a new author");

            var createdAuthor = BaseRequests.DeserializeResponse<Author>(response);
            int expectedAuthorId = int.Parse(_authorRequest._config["NewAuthor:Id"]);
            int expectedBookId = int.Parse(_authorRequest._config["NewAuthor:IdBook"]);
            var expectedFirstName = _authorRequest._config["NewAuthor:FirstName"];
            var expectedLastName = _authorRequest._config["NewAuthor:LastName"];
            
            Assert.AreEqual(expectedAuthorId, createdAuthor.Id, "Author ID does not match the expected value.");
            Assert.AreEqual(expectedBookId, createdAuthor.IdBook, "Author's book ID does not match the expected value.");
            Assert.AreEqual(expectedFirstName, createdAuthor.FirstName, "Author's first name does not match the expected value.");
            Assert.AreEqual(expectedLastName, createdAuthor.LastName, "Author's last name does not match the expected value.");

            var verifyAuthorById = _authorRequest.GetAuthorById(_authorRequest._config["NewAuthor:Id"]);
            BaseRequests.VerifyStatusCode(verifyAuthorById, HttpStatusCode.OK, "Created new author is not successfully saved");

            var author = BaseRequests.DeserializeResponse<Author>(verifyAuthorById);
            Assert.AreEqual(expectedAuthorId, author.Id, "Author ID does not match the expected value.");
            Assert.AreEqual(expectedBookId, author.IdBook, "Author's book ID does not match the expected value.");
            Assert.AreEqual(expectedFirstName, author.FirstName, "Author's first name does not match the expected value.");
            Assert.AreEqual(expectedLastName, author.LastName, "Author's last name does not match the expected value.");
        }

        [Test]
        public void CreateNewAuthorWithInvalidData()
        {
            try
            {
                var response = _authorRequest.PostNewInvalidAuthor();
                BaseRequests.VerifyStatusCode(response, HttpStatusCode.BadRequest, "Unexpectedly succeeded in creating an invalid author");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
            }
            
            // Additional verification to ensure the invalid author does not exist
        }

        [Test]
        public void UpdateExistingAuthorById()
        {
            var response = _authorRequest.UpdateAuthorById(_authorRequest._config["Author:Id"]);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Failed to update an author");

            var updatedAuthor = BaseRequests.DeserializeResponse<Author>(response);
            int expectedAuthorId = int.Parse(_authorRequest._config["UpdatedAuthor:Id"]);
            int expectedBookId = int.Parse(_authorRequest._config["UpdatedAuthor:IdBook"]);
            var expectedFirstName = _authorRequest._config["UpdatedAuthor:FirstName"];
            var expectedLastName = _authorRequest._config["UpdatedAuthor:LastName"];

            Assert.AreEqual(expectedAuthorId, updatedAuthor.Id, "Author ID does not match the expected value.");
            Assert.AreEqual(expectedBookId, updatedAuthor.IdBook, "Author's book ID does not match the expected value.");
            Assert.AreEqual(expectedFirstName, updatedAuthor.FirstName, "Author's first name does not match the expected value.");
            Assert.AreEqual(expectedLastName, updatedAuthor.LastName, "Author's last name does not match the expected value.");
        }

        [Test]
        public void UpdateAuthorByNotExistingId()
        {
            var response = _authorRequest.GetAuthorById(_authorRequest._config["NotExistingAuthor:Id"]);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                response = _authorRequest.UpdateAuthorById(_authorRequest._config["NotExistingAuthor:Id"]);
                BaseRequests.VerifyStatusCode(response, HttpStatusCode.Created, "Failed to create a new author with non-existing ID");
            }
            else
            {
                Console.WriteLine($"Request failed with status code {response.StatusCode}");
            }
        }

        [Test]
        public void DeleteAuthorById()
        {
            var response = _authorRequest.DeleteAuthorById(_authorRequest._config["Author:Id"]);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Failed to delete author");

            response = _authorRequest.GetAuthorById(_authorRequest._config["Author:Id"]);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.NotFound, "Author was not deleted successfully");
        }

        [Test]
        public void DeleteAuthorByNotExistingId()
        {
            var response = _authorRequest.DeleteAuthorById(_authorRequest._config["NotExistingAuthor:Id"]);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.NotFound, "Successfully deleted non-existing author");

            response = _authorRequest.GetAuthorById(_authorRequest._config["NotExistingAuthor:Id"]);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.NotFound, "Failed to delete non-existing author");
        }

        [Test]
        public void DeleteAuthorByInvalidId()
        {
            try
            {
                var response = _authorRequest.DeleteAuthorById(_authorRequest._config["InvalidAuthor:Id"]);

                if (!response.IsSuccessful)
                {
                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                    Console.WriteLine($"Response content: {response.Content}");
                }
                else
                {
                    Console.WriteLine(response.Content);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
            }
        }
    }
}
