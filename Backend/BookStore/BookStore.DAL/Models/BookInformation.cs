using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Models
{
    public class BookInformation
    {
        public int BookId { get; set; } 
        public int InformationTypeId { get; set; } 
        public string Publish { get; set; } // Nhà xuất bản
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual Book Book { get; set; }
        public virtual InformationType InformationType { get; set; }
    }
}
