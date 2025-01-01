using System.Collections.Generic;

namespace IceCreamProject.Models
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public string ShippingAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingCost { get; set; } = 5;
    }
}
