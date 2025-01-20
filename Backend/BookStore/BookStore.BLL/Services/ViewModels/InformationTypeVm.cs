using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services.ViewModels
{
    public class InputInformationTypeVm
    {
        public string Code { get; set; } 
    }
    public class InformationTypeVm
    {
        public int InformationTypeId { get; set; } 
        public string Code { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
