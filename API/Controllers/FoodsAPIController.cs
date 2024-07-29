using Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace API.ApiController
{
    [Route("api/foods")]
    [ApiController]
    public class FoodsAPIController : ControllerBase
    {
        private readonly IAddable<Food> _addsvc;
        private readonly IReadable<Food> _readsvc;
        private readonly ILookupMoreSvc<string, Food> _lookupsvc;
        private readonly ILookupSvc<Guid, Food> _lookupsvc2;    
        private readonly IEditable<Food> _editsvc;
        private readonly IDeletable<Guid, Food> _deletesvc;
        public FoodsAPIController(IAddable<Food> addsvc,
            IReadable<Food> readsvc,
            ILookupMoreSvc<string, Food> lookupsvc,
            IEditable<Food> editsvc,
            IDeletable<Guid, Food> deletesvc,
            IReadable<FoodCategory> foodcategorysvc,
            ILookupSvc<Guid, Food> lookupsvc2)
        {
            _addsvc = addsvc;
            _readsvc = readsvc;
            _lookupsvc = lookupsvc;
            _editsvc = editsvc;
            _deletesvc = deletesvc;
            _lookupsvc2 = lookupsvc2;
        }

        /// <summary>
        /// Lấy danh sách thức ăn
        /// </summary>
        /// <response Code="404">Không tìm thấy</response>
        /// <response Code="200">Tìm thấy</response>
        /// <returns>Danh sách thức ăn</returns>
        [HttpGet]
        public async Task<IEnumerable<Food>> Getfoods()
        {
            return await _readsvc.ReadDatas();
        }

        /// <summary>
        /// Lấy thông tin thức ăn theo foodCode
        /// </summary>
        /// <param name="code">foodCode</param>
        /// <response Code="404">Không tìm thấy</response>
        /// <returns>Thông tin thức ăn</returns>
        [HttpGet("{code}")]
        public async Task<ActionResult<Food>> GetfoodByCode(Guid code)
        {
            var data = await _lookupsvc2.GetDataByKey(code);
            if (data == null)
            {
                return NotFound();
            }
            return data;
        }

        /// <summary>
        /// Lấy thông tin thức ăn theo foodName
        /// </summary>
        /// <param name="key">foodName</param>
        /// <response Code="404">Không tìm thấy</response>
        /// <returns>Thông tin thức ăn</returns>
        [HttpGet("name/{name}")]
        public async Task<ActionResult<IEnumerable<Food>>> GetFoodByName(string name)
        {
            var data = await _lookupsvc.GetListByKey(name);
            if (data == null)
            {
                return NotFound();
            }
            return data.ToList();
        }

        /// <summary>
        /// Chỉnh sửa một thức ăn theo foodCode
        /// </summary>
        /// <response Code="404">Không tìm thấy hoặc foodName đã được sử dụng</response>
        /// <response Code="202">Thành công</response>
        /// <returns>Thức ăn đã chỉnh sửa</returns>
        [HttpPut("{code}")]
        public async Task<IActionResult> PutFood(Guid code, [FromBody] Food food)
        {
            if(code != food.FoodCode)
            {
                return NotFound();
            }
            var data = await _editsvc.EditData(food);
            if (data == null)
            {
                return NotFound();
            }
            return Accepted(data);
        }

        /// <summary>
        /// Thêm một thức ăn mới
        /// </summary>
        /// <remarks>
        /// Lưu ý:
        /// Hãy có một foodCategory trong foodCategories (table database) trước khi thực hiện thêm mới này
        /// Hãy có một admin trong admins (table database) trước khi thực hiện thêm mới này
        /// </remarks>
        /// <example>
        /// {
        ///     "foodName": "Chicken Fried",
        ///     "currentPrice": "25000",
        ///     "left": 100,
        ///     "image": "0394837463.png",
        ///     "fCategoryCode: "..." (mã phân loại),
        ///     "adminCode": "..." (mã quản trị)
        /// }
        /// </example>
        /// <response Code="403">foodName đã tồn tại</response>
        /// <response Code="201">Thành công</response>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostFood([FromBody] Food food)
        {
            var data = await _addsvc.AddNewData(food);
            if(data == null)
            {
                return Forbid();
            }
            return Created();
        }

        /// <summary>
        /// Xóa một thức ăn
        /// </summary>
        /// <param name="code">foodCode</param>
        /// <response Code="200">Thành công</response>
        /// <returns></returns>
        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteFood(Guid code)
        {
            var data = await _deletesvc.DeleteData(code);
            return Ok(data);
        }
    }
}
