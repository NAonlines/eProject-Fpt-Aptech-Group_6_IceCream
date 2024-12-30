using Microsoft.AspNetCore.Identity;
using IceCreamProject.Models;
using Microsoft.EntityFrameworkCore;


namespace IceCreamProject.Models
{
    public class User : IdentityUser
    {
        public bool IsRegistered { get; set; }
        public string? Address { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime? LastForgotPasswordRequest { get; set; }

        public ICollection<MembershipPayment> MembershipPayments { get; set; } = new List<MembershipPayment>();
    }
}
