using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Login
    {
        [Required(ErrorMessage = "Phải nhập email")]
        [EmailAddress(ErrorMessage = "Email phải đúng định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phải nhập password")]
        public string Password { get; set; }
    }
}
