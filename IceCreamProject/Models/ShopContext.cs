using IceCreamProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


public class ShopUser : IdentityUser
{
	// Bạn có thể mở rộng lớp IdentityUser nếu cần
}

public class ShopContext : IdentityDbContext<User, IdentityRole, string>
{
	public ShopContext(DbContextOptions<ShopContext> options) : base(options) { }

	// DbSets cho các model của bạn
	public DbSet<Category> Categories { get; set; }
	public DbSet<Recipe> Recipes { get; set; }
	public DbSet<Book> Books { get; set; }
	public DbSet<Order> Orders { get; set; }
	public DbSet<Feedback> Feedbacks { get; set; }
	public DbSet<MembershipPayment> MembershipPayments { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Checkout> Checkouts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>()
           .Property(b => b.Price)
           .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>()
           .Property(o => o.TotalAmount)
           .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<MembershipPayment>()
           .Property(mp => mp.Amount)
           .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<CartItem>()
           .Property(ci => ci.Price)
           .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Checkout>()
            .Property(c => c.TotalAmount)
            .HasColumnType("decimal(18,2)");
    }



}

