using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [Column(TypeName = "Varchar(200)")]
        public string CustomerEmail { get; set; }

        [ForeignKey("CustomerEmail")]
        public virtual Customer? Customer { get; set; }

        public ICollection<CartItem>? CartItems { get; set; }
    }
}
