using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemId { get; set; }

        public int UnitPrice { get; set; }

        public int Quantity { get; set; }

        public Guid OrderCode { get; set; }

        [ForeignKey("OrderCode")]
        public virtual Order? Order { get; set; }

        public Guid Code { get; set; }

        [ForeignKey("Code")]
        public virtual Food? Food { get; set; }
    }
}
