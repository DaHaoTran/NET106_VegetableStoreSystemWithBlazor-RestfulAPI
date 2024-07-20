using Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/orderitems")]
    [ApiController]
    public class OrderItemsAPIController : ControllerBase
    {
        private readonly IAddable<List<OrderItem>> _addsvc;
        private readonly ILookupMoreSvc<Guid, OrderItem> _lookupsvc;
        public OrderItemsAPIController(IAddable<List<OrderItem>> addsvc,
            ILookupMoreSvc<Guid, OrderItem> lookupsvc)
        {
            _addsvc = addsvc;
            _lookupsvc = lookupsvc;
        }

        /// <summary>
        /// Thêm nhiều thức ăn đặt mới 
        /// </summary>
        /// <remarks>
        /// Lưu ý:
        /// Hãy có một order trong orders (table database) trước khi thực hiện thêm mới này 
        /// Hãy có một food trong foods (table database) trước khi thực hiện thêm mới này 
        /// </remarks>
        /// <example>
        /// [
        ///     {
        ///         "unitPrice": "20000",
        ///         "quantity": "10",
        ///         "orderCode": "..." (mã đơn đặt hàng),
        ///         "foodCode: "..." (mã thức ăn)
        ///     },
        ///     {
        ///         "unitPrice": "40000",
        ///         "quantity": "5",
        ///         "orderCode": "..." (mã đơn đặt hàng),
        ///         "foodCode: "..." (mã thức ăn)
        ///     },
        ///     ...
        /// ]
        /// </example>
        /// <response Code="201">Thành công</response>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostItems([FromBody] List<OrderItem> items)
        {
            var data = await _addsvc.AddNewData(items);
            return Created();
        }

        /// <summary>
        /// Lấy danh sách thức ăn đã đặt theo mã đặt 
        /// </summary>
        /// <param name="code">orderCode</param>
        /// <response Code="404">Không tìm thấy</response>
        /// <returns>Danh sách thức ăn đã đặt</returns>
        [HttpGet("{code}")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetItemsByCode(Guid code)
        {
            var data = await _lookupsvc.GetListByKey(code);
            if (data == null)
            {
                return NotFound();
            }
            return data.ToList();
        }
    }
}
