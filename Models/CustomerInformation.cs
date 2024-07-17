using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Models
{
    public class CustomerInformation
    {
        [Key]
        [AllowNull]
        [DisplayName("Id")]
        public int CInforId { get; set; }

        [MaxLength(500)]
        [Required(ErrorMessage = "Tên người nhận không được bỏ trống")]
        [DisplayName("Tên nhận hàng")]
        public string CustomerName { get; set; }

        [Column(TypeName = "Char(10)")]
        [Required(ErrorMessage = "Số điện thoại không được bỏ trống")]
        [Phone(ErrorMessage = "Số điện thoại chưa đúng")]
        [MinLength(10, ErrorMessage = "Số điện thoại chưa đúng"), MaxLength(10, ErrorMessage = "Số điện thoại chưa đúng")]
        public string PhoneNumber { get; set; }

        [DisplayName("Địa chỉ")]
        public string Address { get; set; }

        [Column(TypeName = "Varchar(200)")]
        [AllowNull]
        [DisplayName("Email tài khoản")]
        public string CustomerEmail { get; set; }

        [ForeignKey("CustomerEmail")]
        public virtual Customer? Customer { get; set; }
    }
}
