namespace OnlineBookstore.main.models;

public class Book
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int PageCount { get; set; }
    public string? Excerpt { get; set; }
    public DateTime PublishDate { get; set; }
    //use string instead DateTime
    
    public Book() { }
    
    public Book (int id, string? title, string? description, int pageCount, string? excerpt, DateTime publishDate)
    {
        Id = id;
        Title = title;
        Description = description;
        PageCount = pageCount;
        Excerpt = excerpt;
        PublishDate = publishDate;
    }
}