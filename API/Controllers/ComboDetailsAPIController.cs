using API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Route("api/combodetails")]
    [ApiController]
    public class ComboDetailsAPIController : ControllerBase
    {
        private readonly IReadable<ComboDetail> _readsvc;
        private readonly IAddable<ComboDetail> _addsvc;
        private readonly IEditable<ComboDetail> _editsvc;
        private readonly IDeletable<int, ComboDetail> _deletesvc;
        private readonly ILookupMoreSvc<Guid, ComboDetail> _lookupsvc;

        public ComboDetailsAPIController(IReadable<ComboDetail> readsvc,
            IAddable<ComboDetail> addsvc,
            IEditable<ComboDetail> editsvc,
            IDeletable<int, ComboDetail> deletesvc,
            ILookupMoreSvc<Guid, ComboDetail> lookupsvc)
        {
            _readsvc = readsvc;
            _addsvc = addsvc;
            _editsvc = editsvc;
            _lookupsvc = lookupsvc;
            _deletesvc = deletesvc;
        }

        /// <summary>
        /// Lấy danh sách chi tiết combo
        /// </summary>
        /// <response Code="404">Không tìm thấy</response>
        /// <response Code="200">Tìm thấy</response>
        /// <returns>Danh sách chi tiết combo</returns>
        [HttpGet]
        public async Task<IEnumerable<ComboDetail>> GetDetails()
        {
            return await _readsvc.ReadDatas();
        }

        /// <summary>
        /// Lấy danh sách chi tiết combo theo foodCode
        /// </summary>
        /// <param name="code">foodCode</param>
        /// <returns>Danh sách chi tiết combo</returns>
        [HttpGet("foodcode/{code}")]
        public async Task<ActionResult<IEnumerable<ComboDetail>>> GetDetailsByFoodCode(Guid code)
        {
            var data = await _lookupsvc.GetListByKey(code);
            if (data == null)
            {
                return NotFound();
            }
            return data.ToList();
        }

        /// <summary>
        /// Thêm một chi tiết combo mới
        /// </summary>
        /// <remarks>
        /// Lưu ý:
        /// Hãy có một food trong foods (table database) trước khi thực hiện thêm mới này
        /// Hãy có một combo trong combos (table database) trước khi thực hiện thêm mới này
        /// </remarks>
        /// <example>
        /// {
        ///     "foodCode": "..." (mã thức ăn),
        ///     "comboCode: "..." (mã combo)
        /// }
        /// </example>
        /// <response Code="403">comboCode có foodCode này đã tồn tại</response>
        /// <response Code="201">Thành công</response>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostDetails([FromBody] ComboDetail detail) 
        {
            var data = await _addsvc.AddNewData(detail);
            if (data == null)
            {
                return Forbid();
            }
            return Created();
        }

        /// <summary>
        /// Sửa một chi tiết combo được chọn theo Id
        /// </summary>
        /// <response Code="404">Không tìm thấy</response>
        /// <response Code="202">Thành công</response>
        /// <returns>Chi tiết Combo đã chỉnh sửa</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetails(int id, [FromBody] ComboDetail detail)
        {
            if(id != detail.Id)
            {
                return NotFound();
            }
            var data = await _editsvc.EditData(detail);
            if (data == null)
            {
                return NotFound();
            }
            return Accepted(data);
        }

        /// <summary>
        /// Xóa một chi tiết combo
        /// </summary>
        /// <param name="id">Id</param>
        /// <response Code="404">Không tìm thấy</response>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetails(int id)
        {
            var data = await _deletesvc.DeleteData(id);
            return Ok(data);
        }
    }
}
