using IceCreamProject.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

public class User : IdentityUser
{
    public bool IsRegistered { get; set; }
    public string Address { get; set; } // Address for order delivery
    public string? ProfileImageUrl { get; set; } // URL for user's profile image
    public ICollection<MembershipPayment> MembershipPayments { get; set; } = new List<MembershipPayment>(); // Navigation property for membership payments
}
