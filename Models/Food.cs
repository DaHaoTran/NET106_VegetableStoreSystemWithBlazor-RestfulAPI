using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Models
{
    public class Food
    {
        [Key]
        [AllowNull]
        public Guid FoodCode { get; set; }

        [MaxLength(300)]
        [Required(ErrorMessage = "Phải nhập tên thức ăn")]
        [DisplayName("Tên thức ăn")]
        public string FoodName { get; set; }

        [Required(ErrorMessage = "Phải nhập giá mới")]
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Giá mới không chứa ký tự")]
        [DisplayName("Giá mới")]
        public int CurrentPrice { get; set; }

        [DisplayName("Giá cũ")]
        [AllowNull]
        public int PreviousPrice { get; set; }

        [Required(ErrorMessage = "Phải nhập số lượng tồn kho")]
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Số lượng tồn kho không chứa ký tự")]
        [DisplayName("Số lượng tồn kho")]
        public int Left { get; set; }

        [RegularExpression(@"^-?\d+$", ErrorMessage = "Số lượng đã bán không chưa ký tự")]
        [AllowNull]
        [DisplayName("Số lượng đã bán")]
        public int Sold { get; set; }

        [Column(TypeName = "Varchar(Max)")]
        [Required(ErrorMessage = "Phải nhập ảnh")]
        [DisplayName("Ảnh")]
        public string Image { get; set; }

        [Required(ErrorMessage = "Phải nhập phân loại thức ăn")]
        [DisplayName("Phân loại")]
        public Guid FCategoryCode { get; set; }

        [ForeignKey("FCategoryCode")]
        public virtual FoodCategory? FoodCategory { get; set; }

        [DisplayName("Mã quản trị")]
        [AllowNull]
        public Guid AdminCode { get; set; }

        [ForeignKey("AdminCode")]
        public virtual Admin? Admin { get; set; }

        public ICollection<CartItem>? CartItems { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }

        public ICollection<ComboDetail>? ComboDetail { get; set; }
    }
}
