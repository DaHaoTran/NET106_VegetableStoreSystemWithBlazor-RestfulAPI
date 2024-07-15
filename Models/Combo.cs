using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Combo
    {
        [Key]
        public Guid ComboCode { get; set; }

        [Key]
        [MaxLength(300)]
        [Required(ErrorMessage = "Combo name is required")]
        public string ComboName { get; set; }

        [Required(ErrorMessage = "Current Price is required")]
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Current Price does't have characters")]
        public int CurrentPrice { get; set; }

        public int PreviousPrice { get; set; }

        [Column(TypeName = "Varchar(Max)")]
        [Required(ErrorMessage = "Image is required")]
        public string Image { get; set; }

        public DateTime ApplyDate { get; set; }
        public DateTime ExpDate { get; set; }

        public ICollection<ComboDetail>? Details { get; set; }
    }
}
