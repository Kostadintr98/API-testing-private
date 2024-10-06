namespace OnlineBookstore.main.models;

public class Book
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? PageCount { get; set; }
    public string? Excerpt { get; set; }
    public string? PublishDate { get; set; }
    
    public Book() { }
    
    public Book (string? id, string? title, string? description, string? pageCount, string? excerpt, string? publishDate)
    {
        Id = id;
        Title = title;
        Description = description;
        PageCount = pageCount;
        Excerpt = excerpt;
        PublishDate = publishDate;
    }
}