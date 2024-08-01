using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Models
{
    public class OrderItem
    {
        [Key]
        public int ItemId { get; set; }

        public int UnitPrice { get; set; }

        public int Quantity { get; set; }

        public Guid OrderCode { get; set; }

        [ForeignKey("OrderCode")]
        public virtual Order? Order { get; set; }

        public Guid FoodCode { get; set; }

        [ForeignKey("FoodCode")]
        public virtual Food? Food { get; set; }
    }
}
