using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services.ViewModels
{
    public class InputBookVm
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        [SkipValidation]
        public decimal OldPrice { get; set; }
        public int StockQuantity { get; set; } = 0;
        public int? CategoryId { get; set; }

        [SkipValidation]
        public string ImageUrl { get; set; }
        public string Author { get; set; }
        [SkipValidation]
        public int Discount { get; set; } = 0;
        public virtual ICollection<InputBookInformationVm> BookInformations { get; set; }
    }
    public class BookVm
    {
        public int BookId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal OldPrice { get; set; }
        public int StockQuantity { get; set; } = 0;
        public int? CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public string Author { get; set; }
        public int Discount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual CategoryVm Category { get; set; }
        public virtual ICollection<BookInformationVm> BookInformations { get; set; }
    }
}
