namespace IceCreamProject.Models
{
    public class Checkout
    {
        public int CheckoutId { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public string ShippingAddress { get; set; }
        public string PhoneNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public List<string> AvailablePaymentMethods { get; set; } = new List<string>();
    
    }
}
