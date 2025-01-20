using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.ViewModels.Authentication
{
    public class RegisterVm
    {
        [Required(ErrorMessage = "Full Name là bắt buộc!")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc!")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number là bắt buộc!")]
        [Phone(ErrorMessage = "Phone number Không hợp lệ.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password là bắt buộc!")]
        [MinLength(6, ErrorMessage = "Password phải có ít nhất 6 ký tự.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[!@#$%^&*()_+={}\[\]:;<>,.?/-]).{6,}$",
        ErrorMessage = "Password phải chứa ít nhất một chữ cái viết hoa, một số và một ký tự đặc biệt.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password là bắt buộc!")]
        [Compare("Password", ErrorMessage = "Passwords không khớp.")]
        public string ConfirmPassword { get; set; }
    }
}
