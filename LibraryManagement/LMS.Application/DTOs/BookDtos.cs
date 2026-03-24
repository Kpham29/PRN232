namespace LMS.Application.DTOs;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public decimal Price { get; set; }
    public int PublishedYear { get; set; }
    public string? ISBN { get; set; }
    public string? Description { get; set; }
    public string? CoverUrl { get; set; }
    public int TotalQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public int PublisherId { get; set; }
    public string PublisherName { get; set; } = "";
}

public class CreateBookDto
{
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public decimal Price { get; set; }
    public int PublishedYear { get; set; }
    public string? ISBN { get; set; }
    public string? Description { get; set; }
    public string? CoverUrl { get; set; }
    public int TotalQuantity { get; set; }
    public int CategoryId { get; set; }
    public int PublisherId { get; set; }
}
