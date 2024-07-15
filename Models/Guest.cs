using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Guest
    {
        [Key]
        public int GuesId { get; set; }

        [MaxLength(500)]
        [Required(ErrorMessage = "Tên người nhận không được bỏ trống")]
        public string GuestName { get; set; }

        [Column(TypeName = "Char(10)")]
        [Required(ErrorMessage = "Số điện thoại không được bỏ trống")]
        [Phone(ErrorMessage = "Số điện thoại chưa đúng")]
        [MinLength(10, ErrorMessage = "Số điện thoại chưa đúng"), MaxLength(10, ErrorMessage = "Số điện thoại chưa đúng")]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public Guid OrderCode { get; set; }

        [ForeignKey("OrderCode")]
        public virtual Order? Order { get; set; }
    }
}
