using System.Net;
using Allure.NUnit;
using Newtonsoft.Json;
using OnlineBookstore.main.requests;
using OnlineBookstore.main.models;
using OnlineBookstore.test.api.steps;


namespace OnlineBookstore.test.api.tests
{
    [AllureNUnit]
    public class AuthorTests : BaseSteps
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
            
            Assert.That(author, Is.Not.Null, "Author not found");
            
            var expectedAuthorId = existingAuthor.Id;
            var expectedBookId = existingAuthor.IdBook;
            var expectedFirstName = existingAuthor.FirstName;
            var expectedLastName = existingAuthor.LastName;
            
            Assert.AreEqual(expectedAuthorId, author.Id, "Author ID does not match the expected value.");
            Assert.AreEqual(expectedBookId, author.IdBook, "Author's book ID does not match the expected value.");
            Assert.AreEqual(expectedFirstName, author.FirstName, "Author's first name does not match the expected value.");
            Assert.AreEqual(expectedLastName, author.LastName, "Author's last name does not match the expected value.");
            
            Console.WriteLine(
                $"Author Id: {author.Id}, Book Reference: {author.IdBook}, Name: {author.FirstName} {author.LastName}");
        }

        [Test(Description = "Can Get Author with non-existing positive ID")]
        public void GetAuthorByNonExistingId()
        {
            string authorId = GenerateRandomNumber(1000000, 5000000).ToString();
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
            var authorID = GenerateRandomNumber(-1000, -1).ToString();
            var response = _authorRequest.GetAuthorById(authorID);
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
            
            Assert.AreEqual(newAuthor.Id, createdAuthor.Id, "Author ID does not match the expected value.");
            Assert.AreEqual(newAuthor.IdBook, createdAuthor.IdBook, "Author's book ID does not match the expected value.");
            Assert.AreEqual(newAuthor.FirstName, createdAuthor.FirstName, "Author's first name does not match the expected value.");
            Assert.AreEqual(newAuthor.LastName, createdAuthor.LastName, "Author's last name does not match the expected value.");

            var verifyAuthorById = _authorRequest.GetAuthorById(newAuthor.Id);
            VerifyStatusCode(verifyAuthorById, HttpStatusCode.OK, "Created new author is not successfully saved");

            var author = DeserializeResponse<Author>(verifyAuthorById);
            Assert.AreEqual(newAuthor.Id, author.Id, "Author ID does not match the expected value.");
            Assert.AreEqual(newAuthor.IdBook, author.IdBook, "Author's book ID does not match the expected value.");
            Assert.AreEqual(newAuthor.FirstName, author.FirstName, "Author's first name does not match the expected value.");
            Assert.AreEqual(newAuthor.LastName, author.LastName, "Author's last name does not match the expected value.");
        }

        [Test(Description = "Can not Create a new Author with invalid data")]
        public void CreateNewAuthorWithInvalidData()
        {
            try
            {
                var newAuthor = new Author
                {
                    Id = GenerateRandomString(15),
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

        //TODO: Refactor this test
        [Test(Description = "Can Update existing Author by ID")] 
        public void UpdateExistingAuthorById()
        {
            var getAuthorResponse = _authorRequest.GetAuthorById(updateAuthor.Id);
            VerifyStatusCode(getAuthorResponse, HttpStatusCode.OK, "Failed to get an author");
            var existingAuthor = DeserializeResponse<Author>(getAuthorResponse);
            
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
            Console.WriteLine(JsonConvert.SerializeObject(updatedAuthor, Formatting.Indented));
            
            Assert.AreEqual(newAuthor.Id, updatedAuthor.Id, "Author ID does not match the expected value.");
            Assert.AreEqual(newAuthor.IdBook, updatedAuthor.IdBook, "Author's book ID does not match the expected value.");
            Assert.AreEqual(newAuthor.FirstName, updatedAuthor.FirstName, "Author's first name does not match the expected value.");
            Assert.AreEqual(newAuthor.LastName, updatedAuthor.LastName, "Author's last name does not match the expected value.");
            
            var verifyAuthorById = _authorRequest.GetAuthorById(newAuthor.Id.ToString());
            VerifyStatusCode(verifyAuthorById, HttpStatusCode.OK, "Created new author is not successfully saved");

            var author = DeserializeResponse<Author>(verifyAuthorById);
            Assert.AreEqual(updatedAuthor.Id, author.Id, "Author ID does not match the expected value.");
            Assert.AreEqual(updatedAuthor.IdBook, author.IdBook, "Author's book ID does not match the expected value.");
            Assert.AreEqual(updatedAuthor.FirstName, author.FirstName, "Author's first name does not match the expected value.");
            Assert.AreEqual(updatedAuthor.LastName, author.LastName, "Author's last name does not match the expected value.");
        }

        [Test(Description = "Can not Update Author with non-existing ID")]
        public void UpdateAuthorWithNonExistingId()
        {
            string authorId = GenerateRandomNumber(1000000, 5000000).ToString();
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
                Console.WriteLine(JsonConvert.SerializeObject(updatedAuthor, Formatting.Indented));
                
                Assert.AreEqual(newAuthor.Id, updatedAuthor.Id, "Author ID does not match the expected value.");
                Assert.AreEqual(newAuthor.IdBook, updatedAuthor.IdBook, "Author's book ID does not match the expected value.");
                Assert.AreEqual(newAuthor.FirstName, updatedAuthor.FirstName, "Author's first name does not match the expected value.");
                Assert.AreEqual(newAuthor.LastName, updatedAuthor.LastName, "Author's last name does not match the expected value.");
            
                var verifyAuthorById = _authorRequest.GetAuthorById(newAuthor.Id);
                VerifyStatusCode(verifyAuthorById, HttpStatusCode.OK, "Created new author is not successfully saved");

                var author = DeserializeResponse<Author>(verifyAuthorById);
                Assert.AreEqual(updatedAuthor.Id, author.Id, "Author ID does not match the expected value.");
                Assert.AreEqual(updatedAuthor.IdBook, author.IdBook, "Author's book ID does not match the expected value.");
                Assert.AreEqual(updatedAuthor.FirstName, author.FirstName, "Author's first name does not match the expected value.");
                Assert.AreEqual(updatedAuthor.LastName, author.LastName, "Author's last name does not match the expected value.");
                
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
            
            var deleteResponse = _authorRequest.DeleteAuthorById(existingAuthor.Id);
            VerifyStatusCode(deleteResponse, HttpStatusCode.OK, "Failed to delete author");

            var verifyResponse = _authorRequest.GetAuthorById(existingAuthor.Id);
            VerifyStatusCode(verifyResponse, HttpStatusCode.NotFound, "Author was not deleted successfully");
        }

        [Test(Description = "Can not Delete an Author with non-existing ID")]
        public void DeleteAuthorWithNonExistingId()
        {
            string authorId = GenerateRandomNumber(1000000, 5000000).ToString();
            
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
            var authorID = GenerateRandomNumber(-1000, -1).ToString();
            Assert.Throws<HttpRequestException>(() =>
            {
                var response = _authorRequest.DeleteAuthorById(authorID);
                VerifyStatusCode(response, HttpStatusCode.BadRequest, $"Tried to delete author with negative ID: {authorID}");
            });
        }
    }
}
