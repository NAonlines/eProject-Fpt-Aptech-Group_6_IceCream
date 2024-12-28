namespace IceCreamProject.Models
{
	public class MembershipPayment
	{
		public int MembershipPaymentId { get; set; } // Non-nullable primary key

		public string UserId { get; set; } // Foreign key to User
		public User User { get; set; } // Navigation property for User

		public string MembershipType { get; set; } // 'Monthly' or 'Yearly'
		public DateTime PaymentDate { get; set; }
		public decimal Amount { get; set; }

		public DateTime MembershipStartDate { get; set; }
		public DateTime MembershipEndDate { get; set; }

		public int? OrdersId { get; set; } // Foreign key to Order (nullable)
		public Order? Order { get; set; } // Navigation property to Order (nullable)

		public string TransactionId { get; set; } // Unique transaction ID for each payment
	}


}
