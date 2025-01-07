namespace IceCreamProject.Models;
public class Book
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; } // Navigation property (nullable)

    // Navigation property for Recipes (one-to-many relationship)
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

}
