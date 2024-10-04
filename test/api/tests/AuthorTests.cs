using System.Net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OnlineBookstore.main.config;
using OnlineBookstore.main.models;
using OnlineBookstore.main.requests;

namespace OnlineBookstore.test.api.tests
{
    public class AuthorTests
    {
        private AuthorRequests _authorRequest;
        private IConfiguration _config;

        [SetUp]
        public void Setup()
        {
            _config = ConfigBuilder.LoadConfiguration();
            _authorRequest = new AuthorRequests(_config);
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
            var response = _authorRequest.GetAuthorById(_config["Author:Id"]);
            
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Failed to retrieve author");
            
            var author = BaseRequests.DeserializeResponse<Author>(response);
            
            Assert.That(author, Is.Not.Null, "Authors have not been found");
            
            int expectedAuthorId = int.Parse(_config["ExpectedAuthor:Id"]);
            int expectedBookId = int.Parse(_config["ExpectedAuthor:BookId"]);
            var expectedFirstName = _config["ExpectedAuthor:FirstName"];
            var expectedLastName = _config["ExpectedAuthor:LastName"];
            
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
            var response = _authorRequest.GetAuthorById(_config["NotExistingAuthor:Id"]);
            
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.NotFound, "Author have been found");

            Console.WriteLine($"Author with ID {_config["NotExistingAuthor:Id"]} have not been found");
        }
        
        [Test] 
        public void GetAuthorByInvalidId()
        {
            try
            {
                var response = _authorRequest.GetAuthorById(_config["InvalidAuthor:Id"]);

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
            int expectedAuthorId = int.Parse(_config["NewAuthor:Id"]);
            int expectedBookId = int.Parse(_config["NewAuthor:IdBook"]);
            var expectedFirstName = _config["NewAuthor:FirstName"];
            var expectedLastName = _config["NewAuthor:LastName"];
            
            Assert.AreEqual(expectedAuthorId, createdAuthor.Id, "Author ID does not match the expected value.");
            Assert.AreEqual(expectedBookId, createdAuthor.IdBook, "Author's book ID does not match the expected value.");
            Assert.AreEqual(expectedFirstName, createdAuthor.FirstName, "Author's first name does not match the expected value.");
            Assert.AreEqual(expectedLastName, createdAuthor.LastName, "Author's last name does not match the expected value.");
            
            var verifyAuthorById = _authorRequest.GetAuthorById(_config["NewAuthor:Id"]);
            BaseRequests.VerifyStatusCode(verifyAuthorById, HttpStatusCode.OK, "Created new author is not successfully saved");
            
            var author = BaseRequests.DeserializeResponse<Author>(verifyAuthorById);
            Assert.AreEqual(expectedAuthorId, author.Id, "Author ID does not match the expected value.");
            Assert.AreEqual(expectedBookId, author.IdBook, "Author's book ID does not match the expected value.");
            Assert.AreEqual(expectedFirstName, author.FirstName, "Author's first name does not match the expected value.");
            Assert.AreEqual(expectedLastName, author.LastName, "Author's last name does not match the expected value.");
        }
        
        [Test] // rework this test because it has to fails not to pass
        public void CreateNewAuthorWithInvalidData()
        {
            Author? createdAuthor;
            try
            {
                var response = _authorRequest.PostNewInvalidAuthor();
                BaseRequests.VerifyStatusCode(response, HttpStatusCode.BadRequest, "Failed to create a new author");
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Request failed: {ex.Message}");
            }
            
            // add verification to check created new author not exists
        }

        [Test]
        public void UpdateExistingAuthorById()
        {
            var response = _authorRequest.UpdateAuthorById(_config["Author:Id"]);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Failed to update an author data");

            var createdAuthor = BaseRequests.DeserializeResponse<Author>(response);
            int expectedAuthorId = int.Parse(_config["UpdatedAuthor:Id"]);
            int expectedBookId = int.Parse(_config["UpdatedAuthor:IdBook"]);
            var expectedFirstName = _config["UpdatedAuthor:FirstName"];
            var expectedLastName = _config["UpdatedAuthor:LastName"];
            
            Assert.AreEqual(expectedAuthorId, createdAuthor.Id, "Author ID does not match the expected value.");
            Assert.AreEqual(expectedBookId, createdAuthor.IdBook, "Author's book ID does not match the expected value.");
            Assert.AreEqual(expectedFirstName, createdAuthor.FirstName, "Author's first name does not match the expected value.");
            Assert.AreEqual(expectedLastName, createdAuthor.LastName, "Author's last name does not match the expected value.");
            
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.Created, "Author have not been created successfully");
            var verifyAuthorById = _authorRequest.GetAuthorById(_config["Author:Id"]);
            BaseRequests.VerifyStatusCode(verifyAuthorById, HttpStatusCode.OK, "Update author data is not successfully saved");
            
            var author = BaseRequests.DeserializeResponse<Author>(verifyAuthorById);
            Assert.AreEqual(expectedAuthorId, author.Id, "Author ID does not match the expected value.");
            Assert.AreEqual(expectedBookId, author.IdBook, "Author's book ID does not match the expected value.");
            Assert.AreEqual(expectedFirstName, author.FirstName, "Author's first name does not match the expected value.");
            Assert.AreEqual(expectedLastName, author.LastName, "Author's last name does not match the expected value.");
        }
        
        [Test]
        public void UpdateAuthorByNotExistingId()
        {
            var response = _authorRequest.GetAuthorById(_config["NotExistingAuthor:Id"]);
            
            Author? author;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                response = _authorRequest.UpdateAuthorByInvalidId(_config["NotExistingAuthor:Id"]);
                BaseRequests.VerifyStatusCode(response, HttpStatusCode.Created, "Author have not been created successfully");
                
                var createdAuthor = BaseRequests.DeserializeResponse<Author>(response);
                int expectedAuthorId = int.Parse(_config["NotExistingAuthor:Id"]);
                int expectedBookId = int.Parse(_config["UpdatedAuthor:IdBook"]);
                var expectedFirstName = _config["UpdatedAuthor:FirstName"];
                var expectedLastName = _config["UpdatedAuthor:LastName"];
                
                Assert.AreEqual(expectedAuthorId, createdAuthor.Id, "Author ID does not match the expected value.");
                Assert.AreEqual(expectedBookId, createdAuthor.IdBook, "Author's book ID does not match the expected value.");
                Assert.AreEqual(expectedFirstName, createdAuthor.FirstName, "Author's first name does not match the expected value.");
                Assert.AreEqual(expectedLastName, createdAuthor.LastName, "Author's last name does not match the expected value.");
                
                var verifyAuthorById = _authorRequest.GetAuthorById(_config["NotExistingAuthor:Id"]);
                BaseRequests.VerifyStatusCode(verifyAuthorById, HttpStatusCode.OK, "Created author is not successfully saved");
            
                author = BaseRequests.DeserializeResponse<Author>(verifyAuthorById);
                Assert.AreEqual(expectedAuthorId, author.Id, "Author ID does not match the expected value.");
                Assert.AreEqual(expectedBookId, author.IdBook, "Author's book ID does not match the expected value.");
                Assert.AreEqual(expectedFirstName, author.FirstName, "Author's first name does not match the expected value.");
                Assert.AreEqual(expectedLastName, author.LastName, "Author's last name does not match the expected value.");
                
                Console.WriteLine("Author have been created and saved successfully");
            } 
            else if (response.StatusCode == HttpStatusCode.OK)
            {
                response = _authorRequest.UpdateAuthorById(_config["NotExistingAuthor:Id"]);
                BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Author have not been created successfully");
                
                var updatedAuthor = BaseRequests.DeserializeResponse<Author>(response);
                int expectedAuthorId = int.Parse(_config["UpdatedAuthor:Id"]);
                int expectedBookId = int.Parse(_config["UpdatedAuthor:IdBook"]);
                var expectedFirstName = _config["UpdatedAuthor:FirstName"];
                var expectedLastName = _config["UpdatedAuthor:LastName"];
                
                Assert.AreEqual(expectedAuthorId, updatedAuthor.Id, "Author ID does not match the expected value.");
                Assert.AreEqual(expectedBookId, updatedAuthor.IdBook, "Author's book ID does not match the expected value.");
                Assert.AreEqual(expectedFirstName, updatedAuthor.FirstName, "Author's first name does not match the expected value.");
                Assert.AreEqual(expectedLastName, updatedAuthor.LastName, "Author's last name does not match the expected value.");
                
                var verifyAuthorById = _authorRequest.GetAuthorById(_config["NotExistingAuthor:Id"]);
                BaseRequests.VerifyStatusCode(verifyAuthorById, HttpStatusCode.OK, "Author update have not been successfully saved");
            
                author = BaseRequests.DeserializeResponse<Author>(verifyAuthorById);
                Assert.AreEqual(expectedAuthorId, author.Id, "Author ID does not match the expected value.");
                Assert.AreEqual(expectedBookId, author.IdBook, "Author's book ID does not match the expected value.");
                Assert.AreEqual(expectedFirstName, author.FirstName, "Author's first name does not match the expected value.");
                Assert.AreEqual(expectedLastName, author.LastName, "Author's last name does not match the expected value.");
                
                Console.WriteLine("Author have been updated and saved successfully");
            }
            else
            {
                Console.WriteLine($"Request failed with status code {response.StatusCode}");
            }
        }

        [Test]
        public void UpdateExistingAuthorWithInvalidData()
        {
            Author? createdAuthor;
            try
            {
                var response = _authorRequest.UpdateExistingAuthorWithInvalidData();
                BaseRequests.VerifyStatusCode(response, HttpStatusCode.BadRequest, "Failed to create a new author");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
            }
            
            // add verification to check created new author not exists
        }
        
        [Test]
        public void UpdateAuthor()
        {
            Author? createdAuthor;
            try
            {
                var response = _authorRequest.PostNewInvalidAuthor();
                BaseRequests.VerifyStatusCode(response, HttpStatusCode.BadRequest, "Failed to create a new author");
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Request failed: {ex.Message}");
            }
            
            // add verification to check created new author not exists
        }
        
        [Test]
        public void DeleteAuthorById()
        {
            var response = _authorRequest.DeleteAuthorById(_config["Author:Id"]);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.OK, "Failed to delete author");
            var author = BaseRequests.DeserializeResponse<Author>(response);
            Assert.IsNull(author, "Authors list is not null");
            Console.WriteLine($"Author with ID {_config["Author:Id"]} have been successfully deleted");
            
            response = _authorRequest.GetAuthorById(_config["Author:Id"]);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.NotFound, "Failed to delete existing author");
        }

        [Test]
        public void DeleteAuthorByNotExistingId()
        {
            var response = _authorRequest.DeleteAuthorById(_config["NotExistingAuthor:Id"]);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.NotFound, "Successfully deleted not existing author");
            
            response = _authorRequest.GetAuthorById(_config["NotExistingAuthor:Id"]);
            BaseRequests.VerifyStatusCode(response, HttpStatusCode.NotFound, "Failed to delete author");
        }

        [Test] 
        public void DeleteAuthorByInvalidId()
        {
            try
            {
                var response = _authorRequest.DeleteAuthorById(_config["InvalidAuthor:Id"]);
                
                if (!response.IsSuccessful)
                {
                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                    Console.WriteLine($"Response content: {response.Content}");
                    return;
                }
                Console.WriteLine(response.Content);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
            }
        }
    }
}
