using OnlineBookstore.main.config;
using OnlineBookstore.main.models;
using RestSharp;

namespace OnlineBookstore.main.requests
{
    public class BookRequests : BaseRequests
    {
        
        private static readonly string? _booksEndpoint;
        
        static BookRequests()
        {
            _booksEndpoint = ConfigBuilder.LoadConfiguration()["API:BooksEndpoint"];
        }
        
        public RestResponse GetAllBooks()
        {
            return ExecuteRequest(_booksEndpoint, Method.Get);
        }

        public RestResponse GetBookById(string? bookId)
        {
            return ExecuteRequest($"{_booksEndpoint}/{bookId}", Method.Get);
        }
        
        public RestResponse PostNewBook(Book book)
        {
            return ExecuteRequest(_booksEndpoint, Method.Post, book);
        }

        public RestResponse PostNewInvalidBook(Book book)
        {
            return ExecuteRequest(_booksEndpoint, Method.Post, book);
        }

        public RestResponse UpdateBookById(string? bookId, Book book)
        {
            return ExecuteRequest($"{_booksEndpoint}/{bookId}", Method.Put, book);
        }

        public RestResponse DeleteBookById(string? bookId)
        {
            return ExecuteRequest($"{_booksEndpoint}/{bookId}", Method.Delete);
        }
    }
}