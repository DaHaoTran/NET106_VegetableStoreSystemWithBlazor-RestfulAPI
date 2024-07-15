using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Food
    {
        [Key]
        public Guid FoodCode { get; set; }

        [Key]
        [MaxLength(300)]
        [Required(ErrorMessage = "Food name is required")]
        public string FoodName { get; set; }

        [Required(ErrorMessage = "Current Price is required")]
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Current Price does't have characters")]
        public int CurrentPrice { get; set; }

        public int PreviousPrice { get; set; }

        [Required(ErrorMessage = "Left is required")]
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Left does't have characters")]
        public int Left { get; set; }

        [RegularExpression(@"^-?\d+$", ErrorMessage = "Sold does't have characters")]
        public int Sold { get; set; }

        [Column(TypeName = "Varchar(Max)")]
        [Required(ErrorMessage = "Image is required")]
        public string Image { get; set; }

        [Required(ErrorMessage = "Food category code is required")]
        public Guid FCategoryCode { get; set; }

        [ForeignKey("FCategoryCode")]
        public virtual FoodCategory? FoodCategory { get; set; }

        public Guid AdminCode { get; set; }

        [ForeignKey("AdminCode")]
        public virtual Admin? Admin { get; set; }

        public ICollection<CartItem>? CartItems { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }

        public ICollection<ComboDetail>? ComboDetail { get; set; }
    }
}
