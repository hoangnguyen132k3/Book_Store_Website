using BookStore.DAL.Models;
using Microsoft.Data.SqlClient.DataClassification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services.ViewModels
{
    public class InputBookInformationVm
    {
        public int InformationTypeId { get; set; }
        public string Publish { get; set; } 
        public virtual InputInformationTypeVm InformationType { get; set; }
    }
    public class BookInformationVm
    {
        public int BookId { get; set; }
        public int InformationTypeId { get; set; }
        public string Publish { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public virtual InformationTypeVm InformationType { get; set; }
    }
}
