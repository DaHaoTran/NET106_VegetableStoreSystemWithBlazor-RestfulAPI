using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Models
{
    public class FoodCategory
    {
        [Key]
        [AllowNull]
        [DisplayName("Mã phân loại")]
        public Guid FCategoryCode { get; set; }

        [MaxLength(200)]
        [Required(ErrorMessage = "Phải nhập tên phân loại")]
        [DisplayName("Tên phân loại")]
        public string CategoryName { get; set; }

        public ICollection<Food>? foods { get; set; }
    }
}
