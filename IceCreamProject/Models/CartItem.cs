namespace IceCreamProject.Models
{
	public class CartItem
	{
		public int CartItemId { get; set; }
		public int ProductId { get; set; } // Linked to BookId or MembershipPaymentId
		public string? Name { get; set; } // Name of the product
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }
		public string? Image { get; set; }
		public int? OrderId { get; set; } // Link to the order

    }
}
