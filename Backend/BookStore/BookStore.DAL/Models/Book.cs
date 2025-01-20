using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace BookStore.DAL.Models
{
    public class Book
    {
        public int BookId { get; set; } 
        public string Name { get; set; } // Tên sách
        public string Description { get; set; } // Mô tả sách
        public decimal Price { get; set; } 
        public decimal OldPrice { get; set; } 
        public int StockQuantity { get; set; } = 0; 
        public int? CategoryId { get; set; } 
        public string ImageUrl { get; set; } 
        public string Author { get; set; } // Tác giả
        public int Discount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now; 
        public DateTime UpdatedAt { get; set; } = DateTime.Now; 

        public virtual Category Category { get; set; } 
        public virtual ICollection<Cart> Carts { get; set; } 
        public virtual ICollection<OrderItem> OrderItems { get; set; } 
        public virtual ICollection<Review> Reviews { get; set; } 
        public virtual ICollection<BookInformation> BookInformations { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? CategoryyId { get; set; }
    }
}
