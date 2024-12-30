using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IceCreamProject.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalAmount { get; set; }
        //public List<OrderDetailViewModel> OrderDetails { get; set; }
    }

    //public class OrderDetailViewModel
    //{
    //    public int OrderDetailId { get; set; }
    //    public int ProductId { get; set; }
    //    public string ProductName { get; set; }
    //    public int Quantity { get; set; }
    //    public decimal UnitPrice { get; set; }
    //    public decimal TotalPrice => Quantity * UnitPrice;
    //}
}
