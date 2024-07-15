﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace Models
{
    public class Admin
    {
        [Key]
        [AllowNull]
        public Guid AdminCode { get; set; }

        [Column(TypeName = "Varchar(200)")]
        [EmailAddress(ErrorMessage = "Email phải đúng định dạng")]
        [Required(ErrorMessage = "Phải nhập Email")]
        public string Email { get; set; }

        [Column(TypeName = "Varchar(100)")]
        [Required(ErrorMessage = "Phải nhập Password")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{5,}$", ErrorMessage = "Password phải chứa 5 ký tự trở lên bao gồm ít nhất 1 số và 1 chữ cái viết hoa !")]
        public string Password { get; set; }

        [AllowNull]
        public bool IsOnl { get; set; }

        [AllowNull]
        public DateTime CreatedDate { get; set; }

        public bool Level {  get; set; }

        public ICollection<Food>? foods { get; set; }
    }
}
