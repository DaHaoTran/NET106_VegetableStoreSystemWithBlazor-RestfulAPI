using Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/cartitems")]
    [ApiController]
    public class CartitemsController : ControllerBase
    {
        private readonly IAddable<CartItem> _addsvc;
        private readonly ILookupMoreSvc<int, CartItem> _lookupsvc;
        private readonly IDeletable<int,  CartItem> _deletesvc;
        private readonly IEditable<CartItem> _editsvc;
        public CartitemsController(IAddable<CartItem> addsvc, 
            ILookupMoreSvc<int, CartItem> lookupsvc, 
            IDeletable<int, CartItem> deletesvc, 
            IEditable<CartItem> editsvc)
        {
            _addsvc = addsvc;
            _lookupsvc = lookupsvc;
            _deletesvc = deletesvc;
            _editsvc = editsvc;
        }

        /// <summary>
        /// Thêm một thức ăn mới vào giỏ hàng
        /// </summary>
        /// <remarks>
        /// Lưu ý:
        /// Hãy có một customer trong customers (table database) để lấy id giỏ hàng trước khi thực hiện thêm mới này
        /// Hãy có một food trong foods (table database) trước khi thực hiện thêm mới này
        /// </remarks>
        /// <example>
        /// {
        ///     "quantity": "2",
        ///     "cartId": "..." (id giỏ hàng),
        ///     "foodCode: "..." (mã thức ăn)
        /// }
        /// </example>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostItem([FromBody] CartItem item)
        {
            var data = await _addsvc.AddNewData(item);
            return Created();
        }

        /// <summary>
        /// Lấy danh sách trong giỏ hàng theo id giỏ hàng
        /// </summary>
        /// <param name="id">cartId</param>
        /// <response name="404">Không tìm thấy</response>
        /// <returns>Danh sách giỏ hàng</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetItems(int id)
        {
            var data = await _lookupsvc.GetListByKey(id);
            if (data == null)
            {
                return NotFound();
            }
            return data.ToList();
        }

        /// <summary>
        /// Xóa một thức ăn trong giỏ hàng
        /// </summary>
        /// <param name="id">itemId</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteitem(int id)
        {
            var data = await _deletesvc.DeleteData(id);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        /// <summary>
        /// Chỉnh sửa một thức ăn trong giỏ hàng theo id
        /// </summary>
        /// <param name="id">itemId</param>
        /// <response name="404">Không tìm thấy</response>
        /// <response name="202">Thành công</response>
        /// <returns>Thức ăn trong giỏ hàng đã chỉnh sửa</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(int id, [FromBody] CartItem item)
        {
            if (id != item.ItemId)
            {
                return NotFound();
            }
            var data = await _editsvc.EditData(item);
            if (data == null)
            {
                return NotFound();
            }
            return Accepted(item);
        }
    }
}
