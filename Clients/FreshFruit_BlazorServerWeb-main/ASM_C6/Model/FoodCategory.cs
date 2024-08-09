using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ASM_C6.Model
{
    public class FoodCategory
    {
        [DisplayName("Mã phân loại")]
        public Guid FCategoryCode { get; set; }

        [MaxLength(200)]
        [Required(ErrorMessage = "Phải nhập tên phân loại")]
        [DisplayName("Tên phân loại")]
        public string CategoryName { get; set; }

    }
}
