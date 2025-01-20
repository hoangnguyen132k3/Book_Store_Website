using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services.ViewModels
{
    public class PaymentVm
    {
        public int UserId { get; set; }  
        public int OrderId { get; set; }  
        public string PaymentMethod { get; set; }  

        public string CardNumber { get; set; }  
        public string CardExpiry { get; set; } 
        public string CardCvc { get; set; }  

        public string PayPalEmail { get; set; }  

        public string FullName { get; set; } 
        public string PhoneNumber { get; set; } 
        public string Commune { get; set; }
        public string Province { get; set; } 
        public string City { get; set; }  
        public string StreetAddress { get; set; }  
        public decimal TotalAmount { get; set; }  
    }
}
