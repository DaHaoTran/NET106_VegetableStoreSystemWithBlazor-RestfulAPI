using API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Route("api/guests")]
    [ApiController]
    public class GuestsAPIController : ControllerBase
    {
        private readonly IReadable<Guest> _readsvc;
        private readonly IAddable<Guest> _addsvc;
        private readonly IDeletable<int, Guest> _deletesvc;
        private readonly ILookupSvc<int, Guest> _lookupsvc;

        public GuestsAPIController(IReadable<Guest> readsvc,
            IAddable<Guest> addsvc,
            IDeletable<int, Guest> deletesvc,
            ILookupSvc<int, Guest> lookupsvc)
        {
            _readsvc = readsvc;
            _addsvc = addsvc;
            _deletesvc = deletesvc;
            _lookupsvc = lookupsvc;
        }


        /// <summary>
        /// Lấy danh sách khách viếng thăm
        /// </summary>
        /// <response name="404">Không tìm thấy</response>
        /// <response name="200">Tìm thấy</response>
        /// <returns>Danh sách khách viếng thăm</returns>
        [HttpGet]
        public async Task<IEnumerable<Guest>> GetGuests()
        {
            return await _readsvc.ReadDatas();
        }

        /// <summary>
        /// Lấy thông tin khách viếng thăm theo id
        /// </summary>
        /// <param name="id">id</param>
        /// <response name="404">Không tìm thấy</response>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Guest>> GetGuestById(int id)
        {
            var data = await _lookupsvc.GetDataByKey(id);
            if (data == null)
            {
                return NotFound();
            }
            return data;
        }

        /// <summary>
        /// Thêm một khách viếng thăm mới
        /// </summary>
        /// <remarks>
        /// Lưu ý:
        /// Hãy có một order trong orders (table database) trước khi thực hiện thêm mới này
        /// </remarks>
        /// <example>
        /// {
        ///     "guestName": "Trần Văn C",
        ///     "phoneNumber": "0291847365",
        ///     "address": "đường Pham Ngũ Lão, TP.HCM",
        ///     "orderCode": "..." (mã đơn hàng của khách hàng đặt)
        /// }
        /// </example>
        /// <response name="201">Thành công</response>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostGuest([FromBody] Guest guest)
        {
            var data = await _addsvc.AddNewData(guest);
            return Created();
        }

        /// <summary>
        /// Xóa một khách viếng thăm
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuest(int id)
        {
            var data = await _deletesvc.DeleteData(id);
            return Ok(data);
        }
    }
}
