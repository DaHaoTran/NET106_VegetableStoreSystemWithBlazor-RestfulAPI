using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ASM_C6.Model
{
    public class Admin
    {
        [DisplayName("Mã quản trị")]
        public Guid AdminCode { get; set; }

        [EmailAddress(ErrorMessage = "Email phải đúng định dạng")]
        [Required(ErrorMessage = "Phải nhập Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phải nhập Password")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{5,}$", ErrorMessage = "Password phải chứa 5 ký tự trở lên bao gồm ít nhất 1 số và 1 chữ cái viết hoa !")]
        public string Password { get; set; }

        [DisplayName("Trạng thái")]
        public bool IsOnl { get; set; }

        [DisplayName("Ngày tạo")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Cấp bậc quản trị")]
        public bool Level { get; set; }
    }
}
