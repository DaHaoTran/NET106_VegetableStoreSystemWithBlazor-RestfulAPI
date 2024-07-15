using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Context;
using API.Services.Interfaces;
using Models;

namespace API.Controllers
{
    [Route("api/admins")]
    [ApiController]
    public class AdminsAPIController : ControllerBase
    {
        private readonly IAddable<Admin> _addsvc;
        private readonly IReadable<Admin> _readsvc;
        private readonly ILookupSvc<string, Admin> _lookupsvc;
        private readonly ILookupSvc<Guid, Admin> _lookupsvc2;
        private readonly IEditable<Admin> _editsvc;
        private readonly IDeletable<Guid, Admin> _deletesvc;
        public AdminsAPIController(IAddable<Admin> addsvc, 
            IReadable<Admin> readsvc, 
            ILookupSvc<string, Admin> lookupsvc, 
            ILookupSvc<Guid, Admin> lookupsvc2, 
            IEditable<Admin> editsvc,
            IDeletable<Guid, Admin> deletesvc)
        {
            _addsvc = addsvc;
            _readsvc = readsvc;
            _lookupsvc = lookupsvc;
            _lookupsvc2 = lookupsvc2;
            _editsvc = editsvc;
            _deletesvc = deletesvc;
        }

        /// <summary>
        /// Lấy danh sách quản trị viên
        /// </summary>
        /// <response Code="404">Không tìm thấy</response>
        /// <response Code="200">Tìm thấy</response>
        /// <returns>Danh sách quản trị viên</returns>
        [HttpGet]
        public async Task<IEnumerable<Admin>> Getadmins()
        {
            return await _readsvc.ReadDatas();
        }

        /// <summary>
        /// Lấy thông tin quản trị theo adminCode
        /// </summary>
        /// <param name="code">mã quản trị</param>
        /// <response Code="404">Không tìm thấy</response>
        /// <response Code="200">Tìm thấy</response>
        /// <returns>Quản trị viên</returns>
        [HttpGet("{code}")]
        public async Task<IActionResult> GetAdmin(Guid code)
        {
            var find = await _lookupsvc2.GetDataByKey(code);
            if(find == null)
            {
                return NotFound();
            }
            return Ok(find);
        }

        /// <summary>
        /// Lấy thông tin quản trị theo email
        /// </summary>
        /// <param name="email">email</param>
        /// <response Code="404">Không tìm thấy</response>
        /// <response Code="200">Tìm thấy</response>
        /// <returns>Quản trị viên</returns>
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetAdminByEmail(string email)
        {
            var find = await _lookupsvc.GetDataByKey(email);
            if (find == null)
            {
                return NotFound();
            }
            return Ok(find);
        }

        /// <summary>
        /// Chỉnh sửa một quản trị được chọn theo adminCode
        /// </summary>
        /// <response Code="404">Không tìm thấy</response>
        /// <response Code="403">Admin có thể IsOnl = true hoặc không tìm thấy</response>
        /// <response Code="202">Thành công</response>
        /// <returns>Quản trị được chỉnh sửa</returns>
        [HttpPut("{code}")]
        public async Task<IActionResult> PutAdmin(Guid code, Admin admin)
        {
            if(code != admin.AdminCode)
            {
                return NotFound();
            }
            var data = await _editsvc.EditData(admin);
            if(data == null)
            {
                return Forbid();
            }
            return Accepted(data);
        }

        /// <summary>
        /// Thêm một quản trị mới
        /// </summary>
        /// <remarks>
        /// mẫu:
        /// {
        ///     email: 'abc1@gmail.com',
        ///     password: 'abc123@',
        ///     level: 'staff',
        /// }
        /// </remarks>
        /// <response Code="404">Không tìm thấy</response>
        /// <response Code="201">Thành công</response>
        /// <returns>Quản trị mới</returns>
        [HttpPost]
        public async Task<IActionResult> PostAdmin(Admin admin)
        {
            var data = await _addsvc.AddNewData(admin);
            if (data == null)
            {
                return Forbid();
            }
            return Created();
        }

        /// <summary>
        /// Xóa một quản trị 
        /// </summary>
        /// <param name="code">mã quản trị</param>
        /// <response Code="404">Không tìm thấy</response>
        /// <returns></returns>
        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteAdmin(Guid code)
        {
            var data = await _deletesvc.DeleteData(code);
            return Ok(data);
        }
    }
}
