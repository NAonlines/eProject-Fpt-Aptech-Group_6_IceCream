using System.ComponentModel.DataAnnotations;

namespace IceCreamProject.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        public int? ParentCategoryId { get; set; } // Parent category ID for hierarchical relationships (nullable)
        public Category? ParentCategory { get; set; } // Navigation property for parent category (nullable)
        public ICollection<Category> ChildCategories { get; set; } = new List<Category>(); // Navigation property for child categories
        public ICollection<Book> Books { get; set; } = new List<Book>(); // Navigation property for books
    }

}
