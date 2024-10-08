﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;

namespace Models
{
    public class Combo
    {
        [Key]
        [AllowNull]
        [DisplayName("Mã combo")]
        public Guid ComboCode { get; set; }

        [MaxLength(300)]
        [Required(ErrorMessage = "Phải nhập tên combo")]
        [DisplayName("Tên combo")]
        public string ComboName { get; set; }

        [Required(ErrorMessage = "Phải nhập giá mới")]
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Current Price does't have characters")]
        [DisplayName("Giá mới")]
        public int CurrentPrice { get; set; }

        [AllowNull]
        [DisplayName("Giá cũ")]
        public int PreviousPrice { get; set; }

        [Column(TypeName = "Varchar(Max)")]
        [Required(ErrorMessage = "Phải nhập ảnh")]
        [DisplayName("Ảnh")]
        public string Image { get; set; }

        //[Required(ErrorMessage = "Phải nhập ngày áp dụng giá mới")]
        [DisplayName("Bắt đầu áp dụng giá mới")]
        [AllowNull]
        public DateTime ApplyDate { get; set; }

        [Required(ErrorMessage = "Phải nhập hạn áp dụng giá mới")]
        [DisplayName("Hạn áp dụng giá mới")]
        public DateTime ExpDate { get; set; }

        public ICollection<ComboDetail>? Details { get; set; }
    }
}
