using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services.ViewModels
{
    public class FilterRequest
    {
        public string Sort { get; set; } 
        public List<string> Categories { get; set; }
        public List<string> Prices { get; set; }
    }
}
