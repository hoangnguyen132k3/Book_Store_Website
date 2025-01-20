using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.ViewModels.Authentication
{
    public class LoginVm
    {
        [Required(ErrorMessage = "Email là bắt buộc!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Cần có Password!")]
        public string Password { get; set; }
    }
}
