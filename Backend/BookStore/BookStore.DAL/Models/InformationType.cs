using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Models
{
    public class InformationType
    {
        public int InformationTypeId { get; set; } 
        public string Code { get; set; } // Mã sách
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<BookInformation> BookInformations { get; set; }
    }
}
