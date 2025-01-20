using System;

namespace BookStore.BLL.Services.ViewModels
{
    public class AddCartVm
    {
        public int UserId { get; set; } 
        public int BookId { get; set; } 
        public int Quantity { get; set; } = 1; 
        public DateTime AddedAt { get; set; } = DateTime.UtcNow; 
        public decimal TotalPrice { get; set; } = 0; 
        public string Status { get; set; } = "Added"; 
    }

    public class CartVm
    {
        public int UserId { get; set; } 
        public int BookId { get; set; } 
        public int Quantity { get; set; } = 1; 
        public DateTime AddedAt { get; set; } = DateTime.UtcNow; 
        public decimal TotalPrice { get; set; } = 0; 
        public string Status { get; set; } = "Added"; 
    }
}
