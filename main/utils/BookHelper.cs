using OnlineBookstore.main.models;

namespace OnlineBookstore.main.utils
{
    public class BookHelper : BaseHelper
    {
        protected Book existingBook => GetBookByType("ExistingBook");
        protected Book updateBook => GetBookByType("UpdateBook");
        protected Book deleteBook => GetBookByType("DeleteBook");

        protected static string GenerateCurrentUtcDate()
        {
            DateTime utcNow = DateTime.UtcNow;
            return utcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
        }
        
        private Book GetBookByType(string bookType)
        {
            return new Book
            {
                Id = _config[$"{bookType}:Id"],
                Title = _config[$"{bookType}:Title"],
                Description = _config[$"{bookType}:Description"],
                PageCount = _config[$"{bookType}:PageCount"],
                Excerpt = _config[$"{bookType}:Excerpt"],
                PublishDate = _config[$"{bookType}:PublishDate"]
            };
        }
    }
}