using IceCreamProject.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace IceCreamProject.Models;
public class Order
{
	public int OrderId { get; set; }
	public DateTime OrderDate { get; set; }
	public string DeliveryAddress { get; set; }
	public string PaymentMethod { get; set; } // 'Credit' or 'Debit'
	public string OrderStatus { get; set; } = "Processing";
	public decimal TotalPrice { get; set; } // Total price of the order
	public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>(); // Navigation property for order details
}