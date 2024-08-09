using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.Forms;

namespace ASM_C6.Model
{
    public class Food
    {
        public Guid FoodCode { get; set; }
        [MaxLength(300)]
        [DisplayName("Tên thức ăn")]
        public string FoodName { get; set; }

        [DisplayName("Giá mới")]
        public int CurrentPrice { get; set; }

        [DisplayName("Giá cũ")]
        [AllowNull]
        public int PreviousPrice { get; set; }

        [DisplayName("Số lượng tồn kho")]
        public int Left { get; set; }
        [AllowNull]
        [DisplayName("Số lượng đã bán")]
        public int Sold { get; set; }

        [Column(TypeName = "Varchar(Max)")]
        [DisplayName("Ảnh")]
        public string Image { get; set; }

        [Required(ErrorMessage = "Phải nhập phân loại thức ăn")]
        [DisplayName("Phân loại")]
        public Guid FCategoryCode { get; set; }

        [DisplayName("Mã quản trị")]
        [AllowNull]
        public Guid AdminCode { get; set; }

        public IBrowserFile BrowserFile { get; set; }

    }
}
