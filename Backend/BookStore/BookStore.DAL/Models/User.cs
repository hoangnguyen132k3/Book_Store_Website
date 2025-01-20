using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Models
{
    public class User : IdentityUser<int>
    {
        public int UserId { get; set; }

        public bool Status { get; set; } = true;
        public string Role { get; set; } = "client";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime LastOnlineAt { get; set; } = DateTime.Now;

        public virtual UserDetails? UserDetails { get; set; } 
        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>(); 
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
