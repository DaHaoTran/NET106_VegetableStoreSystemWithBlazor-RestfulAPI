using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ComboDetail
    {
        [Key]
        public int Id { get; set; }

        public Guid FoodCode { get; set; }

        [ForeignKey("FoodCode")]
        public virtual Food? Food {  get; set; } 

        public Guid ComboCode { get; set; }

        [ForeignKey("ComboCode")]
        public virtual Combo? Combo { get; set; }
    }
}
