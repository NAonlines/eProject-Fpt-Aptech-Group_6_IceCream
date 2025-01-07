using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IceCreamProject.Models
{
   
    public class Memberships
    {
        [Key]
        public int IDMembership { get; set; }
        public int PriceMemberID { get; set; }
        public string UserID { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Status { get; set; }

        [ForeignKey("UserID")]
        public virtual User? UserData { get; set; }

        [ForeignKey("PriceMemberID")]
        public virtual MemberPrice? MemberPriceData { get; set; }
    }
}
