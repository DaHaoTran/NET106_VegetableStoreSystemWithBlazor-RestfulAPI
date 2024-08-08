using API.Services.Interfaces;
using Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/admins")]
    [ApiController]
    public class LoginAdminAPIController : ControllerBase
    {
        private readonly ILoginSvc<Login> _loginSvc;
        public LoginAdminAPIController(ILoginSvc<Login> loginSvc)
        {
            _loginSvc = loginSvc;
        }

        /// <summary>
        /// Đăng nhập site quản trị
        /// </summary>
        /// <response Code="404">Không tìm thấy</response>
        /// <returns>Kết quả đăng nhập</returns>
        [HttpPost("login")]
        public async Task<ActionResult<bool>> LoginAdmin([FromBody] Login admin)
        {
            var data = await _loginSvc.Login(admin);
            if(data == false)
            {
                return NotFound();
            }
            return data;
        }

        /// <summary>
        /// Đăng xuất site quản trị theo email
        /// </summary>
        /// <param name="email">email</param>
        /// <response Code="404">Không tìm thấy</response>
        [HttpGet("logout/{email}")]
        public async Task<ActionResult<bool>> LogoutAdmin(string email)
        {
            var data = await _loginSvc.Logout(email);
            if(data == false)
            {
                return NotFound();
            }
            return data;
        }
    }
}
