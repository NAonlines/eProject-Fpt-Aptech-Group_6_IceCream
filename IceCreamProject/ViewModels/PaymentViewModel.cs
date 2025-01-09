namespace IceCreamProject.ViewModels
{
    public class PaymentViewModel
    {
        public int PriceMemberID { get; set; } // ID của gói Membership
        public string OrderCode { get; set; } // Mã giao dịch
        public decimal PriceRecharge { get; set; } // Giá thanh toán
        public int? OrderId { get; set; } // ID của đơn hàng, nếu có
    }
}
