using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ASM_C6.Model
{
    public class ComboDetail
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Phải nhập thức ăn")]
        [DisplayName("Mã thức ăn")]
        public Guid FoodCode { get; set; }

        [Required(ErrorMessage = "Phải nhập combo")]
        [DisplayName("Mã combo")]
        public Guid ComboCode { get; set; }

    }
}
