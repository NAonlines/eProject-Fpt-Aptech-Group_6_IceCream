﻿using IceCreamProject.Models;
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
    public DbSet<CartItem> CartItems { get; set; }

    
    public DbSet<MemberPrice> MemberPrice { get; set; }
    public DbSet<PaymentMember> PaymentMember { get; set; }
    public DbSet<Memberships> Memberships { get; set; }
    public DbSet<FeedbackResponse> FeedbackResponses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>()
           .Property(b => b.Price)
           .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>()
           .Property(o => o.TotalAmount)
           .HasColumnType("decimal(18,2)");


        modelBuilder.Entity<CartItem>()
           .Property(ci => ci.Price)
           .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<FeedbackResponse>(entity =>
        {
            entity.HasKey(e => e.ResponseId).HasName("PK__Feedback__1AAA646C43F50931");

            entity.Property(e => e.RespondedBy).HasMaxLength(100);
            entity.Property(e => e.RespondedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });
        //modelBuilder.Entity<Checkout>()
        //    .Property(c => c.TotalAmount)
        //    .HasColumnType("decimal(18,2)");
    }



}

