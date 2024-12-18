using System.ComponentModel.DataAnnotations;

namespace IceCreamProject.Models
{
	public class OrderDetail
	{
		public int OrderDetailId { get; set; }
		public int OrderId { get; set; }
		public Order Order { get; set; } // Navigation property
		public int BookId { get; set; }
		public Book Book { get; set; } // Navigation property
		public int Quantity { get; set; }
		public decimal Price { get; set; }
		public decimal Total { get; set; } // Total price for this item (Price * Quantity)
		public int? MembershipPaymentId { get; set; } // Nullable for items that are not memberships
		public MembershipPayment? MembershipPayment { get; set; } // Navigation property for memberships
	}
}
