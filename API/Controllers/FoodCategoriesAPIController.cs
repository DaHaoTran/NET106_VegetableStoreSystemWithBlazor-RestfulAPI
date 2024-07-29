using Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.ApiController
{
    [Route("api/categories")]
    [ApiController]
    public class FoodCategoriesAPIController : ControllerBase
    {
        private readonly IAddable<FoodCategory> _addsvc;
        private readonly IReadable<FoodCategory> _readsvc;
        private readonly ILookupMoreSvc<string, FoodCategory> _lookupsvc;
        private readonly ILookupSvc<Guid, FoodCategory> _lookupsvc2;
        private readonly IEditable<FoodCategory> _editsvc;
        private readonly IDeletable<Guid, FoodCategory> _deletesvc;
        public FoodCategoriesAPIController(IAddable<FoodCategory> addsvc,
            IReadable<FoodCategory> readsvc,
            ILookupMoreSvc<string, FoodCategory> lookupsvc,
            ILookupSvc<Guid, FoodCategory> lookupsvc2,
            IEditable<FoodCategory> editsvc,
            IDeletable<Guid, FoodCategory> deletesvc)
        {
            _addsvc = addsvc;
            _readsvc = readsvc;
            _lookupsvc = lookupsvc;
            _lookupsvc2 = lookupsvc2;
            _editsvc = editsvc;
            _deletesvc = deletesvc;
        }

        /// <summary>
        /// Lấy danh sách phân loại thức ăn
        /// </summary>
        /// <response Code="404">Không tìm thấy</response>
        /// <response Code="200">Tìm thấy</response>
        /// <returns>Danh sách phân loại thức ăn</returns>
        [HttpGet]
        public async Task<IEnumerable<FoodCategory>> GetFoodCategories()
        {
            return await _readsvc.ReadDatas();
        }

        /// <summary>
        /// Lấy thông tin phân loại thức ăn theo fCategoryCode
        /// </summary>
        /// <param name="code">fCategoryCode</param>
        /// <response Code="404">Không tìm thấy</response>
        /// <response Code="200">Tìm thấy</response>
        /// <returns>Phân loại thức ăn</returns>
        [HttpGet("{code}")]
        public async Task<ActionResult<FoodCategory>> GetfoodcategoryByCode(Guid code)
        {
            var data = await _lookupsvc2.GetDataByKey(code);
            if(data == null)
            {
                return NotFound();
            }
            return data;
        }

        /// <summary>
        /// Lấy thông tin phân loại thức ăn theo categoryName
        /// </summary>
        /// <param name="name">categoryName</param>
        /// <response Code="404">Không tìm thấy</response>
        /// <response Code="200">Tìm thấy</response>
        /// <returns>Phân loại thức ăn</returns>
        [HttpGet("categoryname/{name}")]
        public async Task<ActionResult<IEnumerable<FoodCategory>>> GetfoodcategoryByName(string name)
        {
            var data = await _lookupsvc.GetListByKey(name);
            if(data == null)
            {
                return NotFound(); 
            }
            return data.ToList();
        }

        /// <summary>
        /// Chỉnh sửa một loại thức ăn được chọn theo fCategoryCode
        /// </summary>
        /// <response Code="404">Không tìm thấy hoặc categoryName đã được sử dụng</response>
        /// <response Code="202">Thành công</response>
        /// <param name="code">fCategoryCode</param>
        /// <returns>Thức ăn được chỉnh sửa</returns>
        [HttpPut("{code}")]
        public async Task<IActionResult> PutFoodcategory(Guid code, [FromBody] FoodCategory fcate)
        {
            if (code != fcate.FCategoryCode)
            {
                return NotFound();
            }
            var data = await _editsvc.EditData(fcate);
            if (data == null)
            {
                return NotFound();
            }
            return Accepted(data);
        }

        /// <summary>
        /// Thêm một loại thức ăn mới 
        /// </summary>
        /// <example>
        /// {
        ///     "categoryName": "Snacks"
        /// }
        /// </example>
        /// <response Code="404">Không tìm thấy</response>
        /// <response Code="403">categoryName bị trùng</response>
        /// <response Code="201">Thành công</response>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostFoodcategory([FromBody] FoodCategory fcate)
        {
            var data = await _addsvc.AddNewData(fcate);
            if(data == null)
            {
                return Forbid();
            }
            return Created();
        }

        /// <summary>
        /// Xóa một loại thức ăn
        /// </summary>
        /// <param name="code">fCategoryCode</param>
        /// <response Code="404">Không tìm thấy</response>
        /// <returns></returns>
        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteFoodcategory(Guid code)
        {
            var data = await _deletesvc.DeleteData(code);
            return Ok(data);
        }
    }
}
