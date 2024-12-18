namespace IceCreamProject.Models
{
	public class MembershipPayment
	{
		public int MembershipPaymentId { get; set; }
		public string UserId { get; set; }
		public User User { get; set; } // Navigation property
		public string MembershipType { get; set; } // 'Monthly' or 'Yearly'
		public DateTime PaymentDate { get; set; }
		public decimal Amount { get; set; }
		public DateTime MembershipStartDate { get; set; }
		public DateTime MembershipEndDate { get; set; }
	}
}
