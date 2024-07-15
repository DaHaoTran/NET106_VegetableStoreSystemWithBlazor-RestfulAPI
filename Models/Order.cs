﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Order
    {
        [Key]
        public Guid OrderCode { get; set; }

        public DateTime OrderDate { get; set; }

        [MaxLength(100)]
        public string State { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [Column(TypeName = "Nvarchar(Max)")]
        public string? Comment { get; set; }

        [Column(TypeName = "Nvarchar(100)")]
        public string PaymentMethod { get; set; }

        public int Total {  get; set; }

        public int CInforId { get; set; }

        [Column(TypeName = "Varchar(200)")]
        public string CustomerEmail { get; set; }

        [ForeignKey("CustomerEmail")]
        public virtual Customer? Customer { get; set; }

        public ICollection<OrderItem>? Items { get; set; }

        public virtual Guest? Guest { get; set; }
    }
}
