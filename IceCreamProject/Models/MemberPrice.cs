using System.ComponentModel.DataAnnotations;

namespace IceCreamProject.Models
{
    
    public class MemberPrice
    {
        [Key]
        public int IDMemberShipPrice { get; set; }
        public string NamePrice { get; set; }
        public decimal Price { get; set; }
        public string Duration { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
