using System.ComponentModel.DataAnnotations;

namespace LMS.Web.Models;

public class BookVm {
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public int PublisherId { get; set; }
    public string PublisherName { get; set; } = "";
    public decimal Price { get; set; }
    public int PublishedYear { get; set; }
    public string? ISBN { get; set; }
    public int TotalQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public string? CoverUrl { get; set; }
    public string? Description { get; set; }
    public bool CanBorrow => AvailableQuantity > 0;
}

public class BookListVm {
    public List<BookVm> Books { get; set; } = new();
    public List<CategoryVm> Categories { get; set; } = new();
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public int Page { get; set; } = 1;
    public bool HasNext { get; set; }
}

public class BookFormVm {
    public int? Id { get; set; }
    [Required] public string Title { get; set; } = "";
    [Required] public string Author { get; set; } = "";
    [Required][Range(0.01, double.MaxValue)] public decimal Price { get; set; }
    [Required][Range(1000, 2100)] public int PublishedYear { get; set; } = DateTime.Now.Year;
    public string? ISBN { get; set; }
    public string? Description { get; set; }
    public string? CoverUrl { get; set; }
    [Required][Range(1, 9999)] public int TotalQuantity { get; set; } = 1;
    [Required] public int CategoryId { get; set; }
    [Required] public int PublisherId { get; set; }
    public List<CategoryVm> Categories { get; set; } = new();
    public List<PublisherVm> Publishers { get; set; } = new();
}
