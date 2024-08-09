using ASM_C6.Components.Pages.StorePage;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ASM_C6.Model
{
    public class Customer
    {
        
        [Required(ErrorMessage = "Phải nhập email")]
        [EmailAddress(ErrorMessage = "Email phải đúng định dạng")]
        public string Email { get; set; }
        [AllowNull]
        public string PassWord { get; set; }
        [AllowNull]
        [DisplayName("Tên tài khoản")]
        public string UserName { get; set; }

    }
}
