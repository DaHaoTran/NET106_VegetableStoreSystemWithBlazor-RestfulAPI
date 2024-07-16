using Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace API.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersAPIController : ControllerBase
    {
        private readonly IAddable<Customer> _addsvc;
        private readonly IReadable<Customer> _readsvc;
        private readonly ILookupSvc<string, Customer> _lookupsvc;
        private readonly IEditable<Customer> _editsvc;
        private readonly IDeletable<string, Customer> _deletesvc;
        public CustomersAPIController(IAddable<Customer> addsvc,
            IReadable<Customer> readsvc,
            ILookupSvc<string, Customer> lookupsvc,
            IEditable<Customer> editsvc,
            IDeletable<string, Customer> deletesvc)
        {
            _addsvc = addsvc;
            _readsvc = readsvc;
            _lookupsvc = lookupsvc;
            _editsvc = editsvc;
            _deletesvc = deletesvc;
        }

        /// <summary>
        /// Lấy danh sách khách hàng
        /// </summary>
        /// <response name="404">Không tìm thấy</response>
        /// <response name="200">Tìm thấy</response>
        /// <returns>Danh sách khách hàng</returns>
        [HttpGet]
        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            return await _readsvc.ReadDatas();
        }

        /// <summary>
        /// Lấy thông tin khách hàng theo email
        /// </summary>
        /// <param name="email">email</param>
        /// <response name="404">Không tìm thấy</response>
        /// <response name="200">Tìm thấy</response>
        /// <returns>Thông tin khách hàng</returns>
        [HttpGet("{email}")]
        public async Task<ActionResult<Customer>> Getcustomer(string email)
        {
            var data = await _lookupsvc.GetDataByKey(email);
            if (data == null)
            {
                return NotFound();
            }
            return data;
        }

        /// <summary>
        /// Chỉnh sửa một khách hàng được chọn theo email
        /// </summary>
        /// <response name="404">Không tìm thấy</response>
        /// <response name="202">Thành công</response>
        /// <returns>Khách hàng đã chỉnh sửa</returns>
        [HttpPut("{email}")]
        public async Task<IActionResult> PutCustomer(string email, [FromBody] Customer cus)
        {
            if(email != cus.Email)
            {
                return NotFound();
            }
            var data = await _editsvc.EditData(cus);
            if(data == null)
            {
                return NotFound(); 
            }
            return Accepted(data);
        }

        /// <summary>
        /// Thêm một khách hàng mới
        /// </summary>
        /// <remarks>
        /// Mẫu:
        /// {
        ///     email: 'abc1@gmail.com',
        ///     passWord: 'Abc123'
        /// }
        /// </remarks>
        /// <response name="404">Email đã được sử dụng</response>
        /// <response name="201">Thành công</response>
        /// <returns>Khách hàng mới</returns>
        [HttpPost]
        public async Task<IActionResult> PostCustomer([FromBody] Customer cus)
        {
            var data = await _addsvc.AddNewData(cus);
            if (data == null)
            {
                return NotFound();
            }
            return Created();
        }

        /// <summary>
        /// Xóa một khách hàng
        /// </summary>
        /// <param name="email">email</param>
        /// <response name="404">Không tìm thấy</response>
        /// <returns></returns>
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteCustomer(string email)
        {
            var data = await _deletesvc.DeleteData(email);
            return Ok(data);
        }
    }
}
