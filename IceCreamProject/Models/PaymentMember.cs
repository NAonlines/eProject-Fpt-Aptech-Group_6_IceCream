using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IceCreamProject.Models
{
    
    public class PaymentMember
    {
        [Key]
        public int IDPayment { get; set; }
        public string PaymentCode { get; set; }
        public decimal Price { get; set; }

        public string UserID { get; set; }
        public int PriceMemberID { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Status { get; set; }

        [ForeignKey("UserID")]
        public virtual User? UserData { get; set; }

        [ForeignKey("PriceMemberID")]
        public virtual MemberPrice? MemberPriceData { get; set; }
        public int? OrderId { get; set; }

    }
}
