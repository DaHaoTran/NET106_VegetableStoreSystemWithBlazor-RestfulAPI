using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ASM_C6.Model
{
    public class OrderItem
    {

        public int UnitPrice { get; set; }

        public int Quantity { get; set; }

        public Guid OrderCode { get; set; }

        public Guid FoodCode { get; set; }


    }
}
