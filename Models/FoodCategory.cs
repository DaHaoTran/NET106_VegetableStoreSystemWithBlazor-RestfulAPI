using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class FoodCategory
    {
        [Key]
        public Guid FCategoryCode { get; set; }

        [Key]
        [MaxLength(200)]
        [Required(ErrorMessage = "Category name is required")]
        public string CategoryName { get; set; }

        public ICollection<Food>? foods { get; set; }
    }
}
