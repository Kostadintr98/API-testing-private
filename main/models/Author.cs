namespace OnlineBookstore.main.models;

public class Author
{
    public int Id { get; set; }
    public int IdBook { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    public Author() { }
    
    public Author(int id, int idBook, string? firstName, string? lastName)
    {
        Id = id;
        IdBook = idBook;
        FirstName = firstName;
        LastName = lastName;
    }
}

public class InvalidAuthor
{
    public string? Id { get; set; }
    public string? IdBook { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}