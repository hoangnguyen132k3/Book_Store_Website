using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services.ViewModels
{
    public class CategoryVm
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public int? BookCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
       
    }
    public class InputCategoryVm
    {
        public string Name { get; set; }
        [SkipValidation]
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
