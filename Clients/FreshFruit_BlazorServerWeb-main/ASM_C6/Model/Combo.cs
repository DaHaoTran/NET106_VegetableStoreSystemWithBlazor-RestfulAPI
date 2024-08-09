using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.Forms;

namespace ASM_C6.Model
{
    public class Combo
    {
        [DisplayName("Mã combo")]
        public Guid ComboCode { get; set; }

        [MaxLength(300)]
        [Required(ErrorMessage = "Phải nhập tên combo")]
        [DisplayName("Tên combo")]
        public string ComboName { get; set; }

        [Required(ErrorMessage = "Phải nhập giá cho combo")]
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Current Price does't have characters")]
        [DisplayName("Giá combo")]
        public int CurrentPrice { get; set; }

        [AllowNull]
        [DisplayName("Giá cũ")]
        public int PreviousPrice { get; set; }

        [Column(TypeName = "Varchar(Max)")]
        [DisplayName("Ảnh")]
        public string Image { get; set; }

        //[Required(ErrorMessage = "Phải nhập ngày áp dụng giá mới")]
        [DisplayName("Bắt đầu áp dụng giá mới")]
        [AllowNull]
        public DateTime ApplyDate { get; set; }

        [Required(ErrorMessage = "Phải nhập hạn áp dụng giá mới")]
        [DisplayName("Hạn áp dụng giá mới")]
        public DateTime ExpDate { get; set; }
        public IBrowserFile BrowserFile { get; set; }
    }
}
