using Models;
using API.Services.Implement;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersAPIController : ControllerBase
    {
        private readonly ILookupMoreSvc<string, Order> _lookupSvc;
        private readonly IEditable<Order> _editsvc;
        private readonly IReadable<Order> _readsvc;
        private readonly IAddable<Order> _addsvc;
        private readonly IDeletable<Guid, Order> _deletesvc; 
        public OrdersAPIController(IReadable<Order> readsvc,
            ILookupMoreSvc<string, Order> lookupsvc,
            IEditable<Order> editsvc,
            IDeletable<Guid, Order> deletesvc,
            IAddable<Order> addsvc) 
        {
            _readsvc = readsvc;
            _lookupSvc = lookupsvc;
            _editsvc = editsvc;
            _deletesvc = deletesvc;  
            _addsvc = addsvc;
        }

        /// <summary>
        /// Lấy danh sách đơn đặt 
        /// </summary>
        /// <response Code="404">Không tìm thấy</response>
        /// <response Code="200">Tìm thấy</response>
        /// <returns>Danh sách đơn đặt</returns>
        [HttpGet]
        public async Task<IEnumerable<Order>> Getorders()
        {
            return await _readsvc.ReadDatas();
        }

        /// <summary>
        /// Lấy thông tin đơn hàng theo email
        /// </summary>
        /// <param name="email">email</param>
        /// <response Code="404">Không tìm thấy</response>
        /// <returns>Thông tin đơn hàng</returns>
        [HttpGet("email/{email}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByEmail(string email)
        {
            var data = await _lookupSvc.GetListByKey(email);
            if (data == null)
            {
                return NotFound();
            }
            return data.ToList();
        }

        /// <summary>
        /// Thêm một đơn hàng mới
        /// </summary>
        /// <remarks>
        /// Lưu ý:
        /// Hãy có một customerInformation trong customerInformations (table database) trước khi thực hiện thêm mới này.
        /// Hãy có một customer trong customers (table database) trước khi thực hiện thêm mới này.
        /// State có 3 loại: Not delivered (chưa giao), Ongoing delivered (đang giao), Delivered (đã giao)
        /// </remarks>
        /// <example>
        /// {
        ///     "state": "Not Delivered",
        ///     "comment": "test data",
        ///     "paymentMethod": "Thanh toán khi nhận hàng",
        ///     "total": "200000",
        ///     "cInforId": "..." (mã địa chỉ khách hàng),
        ///     "customerEmail": "..." (email tài khoản khách hàng)
        /// }
        /// </example>
        /// <response Code="201">Thành công</response>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostOrder([FromBody] Order order)
        {
            var data = await _addsvc.AddNewData(order);
            return Created();
        }

        /// <summary>
        /// Chỉnh sửa trạng thái đơn hàng theo mã đơn hàng
        /// </summary>
        /// <remarks>
        /// State có 3 loại: Not delivered (chưa giao), Ongoing delivered (đang giao), Delivered (đã giao)
        /// </remarks>
        /// <param name="code">orderCode</param>
        /// <response Code="404">Không tìm thấy</response>
        /// <response Code="202">Thành công</response>
        /// <returns>Đơn hàng đã chỉnh sửa</returns>
        [HttpPut("{code}")]
        public async Task<IActionResult> PutOrder(Guid code, [FromBody] Order order)
        {
            var data = await _editsvc.EditData(order);
            if (data == null)
            {
                return NotFound();
            }
            return Accepted(data);
        }

        /// <summary>
        /// Xóa một đơn hàng 
        /// </summary>
        /// <param name="code">orderCode</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteOrder(Guid code)
        {
            var data = await _deletesvc.DeleteData(code);
            return Ok(data);
        }
    }
}
