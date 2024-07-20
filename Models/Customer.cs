using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Models
{
    public class Customer
    {
        [Key]
        [Column(TypeName = "Varchar(200)")]
        [Required(ErrorMessage = "Phải nhập email")]
        [EmailAddress(ErrorMessage = "Email phải đúng định dạng")]
        public string Email { get; set; }

        [Column(TypeName = "Varchar(100)")]
        [Required(ErrorMessage = "Phải nhập password")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{5,}$", ErrorMessage = "Password phải chứa 5 ký tự trở lên bao gồm ít nhất 1 số và 1 chữ cái viết hoa !")]
        public string PassWord { get; set; }

        //[NotMapped]
        //[Required(ErrorMessage = "Confirm password is required")]
        //[Column(TypeName = "Varchar(100)")]
        //[Compare(nameof(PassWord), ErrorMessage = "Confirm password is not same password")]
        //public string ConfirmPassWord { get; set; }

        [Column(TypeName = "Varchar(300)")]
        [AllowNull]
        [DisplayName("Tên tài khoản")]
        public string UserName { get; set; }

        public ICollection<CustomerInformation>? CustomerInformations { get; set; }

        public virtual Cart? Cart { get; set; }

        public ICollection<Order>? orders { get; set; }   
    }
}
