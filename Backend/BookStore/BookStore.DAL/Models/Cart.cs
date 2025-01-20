using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Models
{
    public class Cart
    {
        [Key]
        [Column(Order = 1)]
        public int UserId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int BookId { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal TotalPrice { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Added";

        public virtual User? User { get; set; }
        public virtual Book? Book { get; set; }
    }
}
