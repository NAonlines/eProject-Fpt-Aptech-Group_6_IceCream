namespace IceCreamProject.Models;
public class Recipe
{
    public int RecipeId { get; set; }
    public string Name { get; set; }
    public string Ingredients { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsApproved { get; set; } = false; // Needs admin approval
    public int? CategoryId { get; set; }
    public Category? Category { get; set; } // Navigation property (nullable)
    public string? CreatedById { get; set; } // User ID of the creator
    public User? CreatedBy { get; set; } // Navigation property to User

    public int? BookId { get; set; }
    public Book? Book { get; set; } // Navigation property (nullable)
}
