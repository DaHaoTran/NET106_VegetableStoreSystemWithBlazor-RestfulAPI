using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class CartItem
    {
        [Key]
        public int ItemId { get; set; }

        public int Quantity { get; set; }

        public int CartId { get; set; }

        [ForeignKey("CartId")]
        public virtual Cart? Cart { get; set; }

        public Guid Code { get; set; }

        [ForeignKey("Code")]
        public virtual Food? Food { get; set; }

    }
}
