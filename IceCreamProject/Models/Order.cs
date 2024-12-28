using IceCreamProject.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace IceCreamProject.Models;
public class Order
{
	public int OrderId { get; set; }
	public string CustomerName { get; set; }
	public string Address { get; set; }
	public decimal TotalAmount { get; set; }
	public string PaymentMethod { get; set; }
	public DateTime OrderDate { get; set; }

	// Foreign key to link with User (Account)
	public string UserId { get; set; }
	public User User { get; set; }

	// List of items in the cart
	public List<CartItem> CartItems { get; set; } = new List<CartItem>();
}