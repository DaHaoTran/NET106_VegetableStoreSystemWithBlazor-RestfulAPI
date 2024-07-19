using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Models
{
    public class CartItem
    {
        [Key]
        [DisplayName("Id")]
        [AllowNull]
        public int ItemId { get; set; }

        [DisplayName("Số lượng")]
        [AllowNull]
        public int Quantity { get; set; }

        [DisplayName("Id giỏ hàng")]
        [AllowNull]
        public int CartId { get; set; }

        [ForeignKey("CartId")]
        public virtual Cart? Cart { get; set; }

        [DisplayName("Mã thức ăn")]
        [AllowNull]
        public Guid FoodCode { get; set; }

        [ForeignKey("FoodCode")]
        public virtual Food? Food { get; set; }

    }
}
