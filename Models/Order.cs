using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Models
{
    public class Order
    {
        [Key]
        [DisplayName("Mã đơn")]
        public Guid OrderCode { get; set; }

        [DisplayName("Ngày đặt")]
        public DateTime OrderDate { get; set; }

        [MaxLength(100)]
        [DisplayName("Trạng thái")]
        [AllowNull]
        public string State { get; set; }

        [DisplayName("Ngày giao")]
        public DateTime? DeliveryDate { get; set; }

        [Column(TypeName = "Nvarchar(Max)")]
        [DisplayName("Ghi chú")]
        public string? Comment { get; set; }

        [Column(TypeName = "Nvarchar(100)")]
        [DisplayName("Phương thức thanh toán")]
        public string PaymentMethod { get; set; }

        [DisplayName("Tổng tiền")]
        public int Total {  get; set; }

        [DisplayName("Mã địa chỉ")]
        public int CInforId { get; set; }

        [Column(TypeName = "Varchar(200)")]
        [DisplayName("Email khách hàng")]
        public string CustomerEmail { get; set; }

        [ForeignKey("CustomerEmail")]
        public virtual Customer? Customer { get; set; }

        public ICollection<OrderItem>? Items { get; set; }

        public virtual Guest? Guest { get; set; }
    }
}
