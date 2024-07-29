using API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Models;

namespace API.Controllers
{
    [Route("api/combos")]
    [ApiController]
    public class CombosAPIController : ControllerBase
    {
        private readonly IAddable<Combo> _addsvc;
        private readonly IEditable<Combo> _editsvc;
        private readonly IReadable<Combo> _readsvc;
        private readonly IDeletable<Guid, Combo> _deletesvc;
        private readonly ILookupSvc<Guid, Combo> _lookupsvc;
        private readonly ILookupMoreSvc<string, Combo> _lookupsvc2;

        public CombosAPIController(IAddable<Combo> addsvc, 
            IEditable<Combo> editsvc, 
            IReadable<Combo> readsvc, 
            IDeletable<Guid, Combo> deletesvc, 
            ILookupSvc<Guid, Combo> lookupsvc, 
            ILookupMoreSvc<string, Combo> lookupsvc2)
        {
            _addsvc = addsvc;
            _editsvc = editsvc;
            _readsvc = readsvc;
            _deletesvc = deletesvc;
            _lookupsvc = lookupsvc;
            _lookupsvc2 = lookupsvc2;
        }

        /// <summary>
        /// Lấy danh sách combo
        /// </summary>
        /// <response Code="404">Không tìm thấy</response>
        /// <response Code="200">Tìm thấy</response>
        /// <returns>Danh sách combo</returns>
        [HttpGet]
        public async Task<IEnumerable<Combo>> GetCombos()
        {
            return await _readsvc.ReadDatas();
        }

        /// <summary>
        /// Lấy thông tin combo theo comboCode
        /// </summary>
        /// <param name="code">comboCode</param>
        /// <response Code="404">Không tìm thấy</response>
        /// <returns>Thông tin combo</returns>
        [HttpGet("{code}")]
        public async Task<ActionResult<Combo>> GetComboByCode(Guid code)
        {
            var data = await _lookupsvc.GetDataByKey(code);
            if(data == null)
            {
                return NotFound();
            }
            return data;
        }

        /// <summary>
        /// Lấy thông tin combo theo comboName
        /// </summary>
        /// <param name="name">comboName</param>
        /// <response Code="404">Không tìm thấy</response>
        /// <returns>Thông tin combo</returns>
        [HttpGet("name/{name}")]
        public async Task<ActionResult<IEnumerable<Combo>>> GetComboByName(string name)
        {
            var data = await _lookupsvc2.GetListByKey(name);
            if(data == null)
            {
                return NotFound();
            }
            return data.ToList();
        }

        /// <summary>
        /// Thêm một combo mới
        /// </summary>
        /// <example>
        /// {
        ///     "comboName": "Family Combo",
        ///     "currentPrice: "100000",
        ///     "image": "9304938273.jpg",
        ///     "expDate: "30/08/2024 12:48:32 CH"
        /// }
        /// </example>
        /// <response Code="403">comboName đã được sử dụng</response>
        /// <resposne Code="201">Thành công</resposne>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostCombo([FromBody] Combo combo)
        {
            var data = await _addsvc.AddNewData(combo);
            if(data == null)
            {
                return Forbid();
            }
            return Created();
        }

        /// <summary>
        /// Sửa một combo được chọn theo comboCode
        /// </summary>
        /// <param name="code">comboCode</param>
        /// <response Code="404">Không tìm thấy hhoặc comName đã được sử dụng</response>
        /// <response Code="202">Thành công</response>
        /// <returns>Combo đã chỉnh sửa</returns>
        [HttpPut("{code}")]
        public async Task<IActionResult> PutCombo(Guid code, [FromBody] Combo combo)
        {
            if(code != combo.ComboCode)
            {
                return NotFound();
            }
            var data = await _editsvc.EditData(combo);
            if (data == null)
            {
                return NotFound(); 
            }
            return Accepted(data);
        }

        /// <summary>
        /// Xóa một combo
        /// </summary>
        /// <param name="code">comboCode</param>
        /// <response Code="404">Không tìm thấy</response>
        /// <returns></returns>
        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteCombo(Guid code)
        {
            var data = await _deletesvc.DeleteData(code);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
    }
}
