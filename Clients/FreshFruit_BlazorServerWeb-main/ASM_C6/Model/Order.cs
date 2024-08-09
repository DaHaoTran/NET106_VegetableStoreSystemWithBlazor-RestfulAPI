using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ASM_C6.Model
{
    public class Order
    {
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
        public int Total { get; set; }

        [Column(TypeName = "Varchar(200)")]
        [DisplayName("Email khách hàng")]
        public string CustomerEmail { get; set; }

        [DisplayName("Mã địa chỉ")]
        public int CInforId { get; set; }

    }
}
