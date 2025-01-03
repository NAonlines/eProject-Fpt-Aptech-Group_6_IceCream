namespace IceCreamProject.Models
{
    public class CheckoutDTO
    {
        public List<CartItem> Cart { get; set; }
        public User User { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }
    }

}
