namespace OnlineBookstore.main.models;

public class Author
{
    public string? Id { get; set; }
    public string? IdBook { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    public Author() { }
    
    public Author(string? id, string? idBook, string? firstName, string? lastName)
    {
        Id = id;
        IdBook = idBook;
        FirstName = firstName;
        LastName = lastName;
    }
}