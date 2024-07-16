using Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/infor/customers")]
    [ApiController]
    public class CustomerInformationsAPIController : ControllerBase
    {
        private readonly ILookupSvc<int, CustomerInformation> _lookupSvc;
        private readonly IAddable<CustomerInformation> _addsvc;
        private readonly IReadable<CustomerInformation> _readsvc;
        private readonly IDeletable<int, CustomerInformation> _deletesvc;
        private readonly IEditable<CustomerInformation> _editsvc;
        private readonly ILookupMoreSvc<string, CustomerInformation> _lookupSvc2;
        public CustomerInformationsAPIController(ILookupSvc<int, CustomerInformation> lookupSvc,
                IAddable<CustomerInformation> addsvc,
                IReadable<CustomerInformation> readsvc,
                IDeletable<int, CustomerInformation> deletesvc,
                IEditable<CustomerInformation> editsvc,
                ILookupMoreSvc<string, CustomerInformation> lookupSvc2)
        {
            _lookupSvc = lookupSvc;
            _lookupSvc2 = lookupSvc2;
            _addsvc = addsvc;
            _readsvc = readsvc;
            _deletesvc = deletesvc;
            _editsvc = editsvc;
        }

        /// <summary>
        /// Thêm một thông tin khách hàng mới
        /// </summary>
        /// <remarks>
        /// Mẫu:
        /// {
        ///     "customerName": "Trần Văn B",
        ///     "phoneNumber": "0394857621",
        ///     "address": "Công viên phần mềm Quang Trung",
        ///     "customerEmail": "..." (email tài khoản khách hàng đã tạo)
        ///  }
        /// </remarks>
        /// <response name="404">Không tìm thấy tài khoản có email trùng khớp</response>
        /// <response name="201">Thành công</response>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostInformation([FromBody] CustomerInformation information)
        {
            var data = await _addsvc.AddNewData(information);
            if(data == null)
            {
                return NotFound();
            }
            return Created();
        }

        /// <summary>
        /// Lấy danh sách thông tin khách hàng
        /// </summary>
        /// <response name="404">Không tìm thấy</response>
        /// <response name="200">Tìm thấy</response>
        /// <returns>Danh sách thông tin khách hàng</returns>
        [HttpGet]
        public async Task<IEnumerable<CustomerInformation>> GetInformations()
        {
            return await _readsvc.ReadDatas();
        }

        /// <summary>
        /// Lấy một danh sách thông tin khách hàng theo email
        /// </summary>
        /// <param name="email">email</param>
        /// <response name="404">Không tìm thấy</response>
        /// <returns>Danh sách thông tin khách hàng</returns>
        [HttpGet("email/{email}")]
        public async Task<ActionResult<IEnumerable<CustomerInformation>>> GetInformationByEmail(string email)
        {
            var data = await _lookupSvc2.GetListByKey(email);
            if(data == null)
            {
                return NotFound();
            }
            return data.ToList();
        }

        /// <summary>
        /// Lấy thông tin khách hàng theo id
        /// </summary>
        /// <param name="id">cInforId</param>
        /// <returns>Thông tin khách hàng</returns>s
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerInformation>> GetInformationByCode(int id)
        {
            var data = await _lookupSvc.GetDataByKey(id);
            if (data == null)
            {
                return NotFound();
            }
            return data;
        }

        /// <summary>
        /// Sửa một thông tin khách hàng theo id
        /// </summary>
        /// <param name="id">cInforId</param>
        /// <response name="404">Không tìm thấy</response>
        /// <response name="202">Thành công</response>
        /// <returns>Thông tin khách hàng đã sửa</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInformation(int id, [FromBody] CustomerInformation information)
        {
            if(id != information.CInforId)
            {
                return NotFound();
            }
            var data = await _editsvc.EditData(information);
            if (data == null)
            {
                return NotFound();
            }
            return Accepted(data);
        }

        /// <summary>
        /// Xóa một thông tin khách hàng 
        /// </summary>
        /// <param name="id">cInforId</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInformation(int id)
        {
            var data = await _deletesvc.DeleteData(id);
            return Ok(data);
        }
    }
}
