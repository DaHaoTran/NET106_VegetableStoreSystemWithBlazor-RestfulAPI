using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ComboDetail
    {
        [Key]
        [AllowNull]
        public int Id { get; set; }

        [Required(ErrorMessage = "Phải nhập thức ăn")]
        [DisplayName("Mã thức ăn")]
        public Guid FoodCode { get; set; }

        [ForeignKey("FoodCode")]
        public virtual Food? Food {  get; set; }

        [Required(ErrorMessage = "Phải nhập combo")]
        [DisplayName("Mã combo")]
        public Guid ComboCode { get; set; }

        [ForeignKey("ComboCode")]
        public virtual Combo? Combo { get; set; }
    }
}
