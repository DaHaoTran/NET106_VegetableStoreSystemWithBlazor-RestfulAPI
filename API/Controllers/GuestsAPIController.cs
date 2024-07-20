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
        private readonly ILookupSvc<string, Guest> _lookupsvc2;
        private readonly ILookupMoreSvc<string, Guest> _lookupsvc3;

        public GuestsAPIController(IReadable<Guest> readsvc,
            IAddable<Guest> addsvc,
            IDeletable<int, Guest> deletesvc,
            ILookupSvc<int, Guest> lookupsvc,
            ILookupSvc<string, Guest> lookupsvc2,
            ILookupMoreSvc<string, Guest> lookupsvc3)
        {
            _readsvc = readsvc;
            _addsvc = addsvc;
            _deletesvc = deletesvc;
            _lookupsvc = lookupsvc;
            _lookupsvc2 = lookupsvc2;
            _lookupsvc3 = lookupsvc3;
        }


        /// <summary>
        /// Lấy danh sách khách viếng thăm
        /// </summary>
        /// <response Code="404">Không tìm thấy</response>
        /// <response Code="200">Tìm thấy</response>
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
        /// <response Code="404">Không tìm thấy</response>
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
        /// Lấy thông tin khách viếng thăm theo phoneNumber
        /// </summary>
        /// <param name="phone">phoneNumber</param>
        /// <response Code="404">Không tìm thấy</response>
        /// <returns></returns>
        [HttpGet("phone/{phone}")]
        public async Task<ActionResult<Guest>> GetGuestByPhoneNum(string phone)
        {
            var data = await _lookupsvc2.GetDataByKey(phone);
            if (data == null)
            {
                return NotFound();
            }
            return data;
        }

        /// <summary>
        /// Lấy thông tin khách viếng thăm theo guestName, address
        /// </summary>
        /// <param name="related">related information</param>
        /// <response Code="404">Không tìm thấy</response>
        /// <returns></returns>
        [HttpGet("relatedinformation/{related}")]
        public async Task<ActionResult<IEnumerable<Guest>>> GetGuestByInfor(string related)
        {
            var data = await _lookupsvc3.GetListByKey(related);
            if (data == null)
            {
                return NotFound();
            }
            return data.ToList();
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
        /// <response Code="201">Thành công</response>
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
