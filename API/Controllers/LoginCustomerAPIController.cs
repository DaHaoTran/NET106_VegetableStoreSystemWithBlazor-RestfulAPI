using API.Services.Interfaces;
using Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class LoginCustomerAPIController : ControllerBase
    {
        private readonly ILoginSvc<Customer> _loginSvc;
        public LoginCustomerAPIController(ILoginSvc<Customer> loginSvc)
        {
            _loginSvc = loginSvc;
        }

        /// <summary>
        /// Đăng nhập site khách hàng
        /// </summary>
        /// <response Code="404">Không tìm thấy</response>
        /// <returns>Kết quả đăng nhập</returns>
        [HttpPost("login")]
        public async Task<ActionResult<bool>> LoginCustomer([FromBody] Customer customer)
        {
            var data = await _loginSvc.Login(customer);
            if (data == false)
            {
                return NotFound();
            }
            return data;
        }

        /// <summary>
        /// Đăng xuất site khách hàng theo email
        /// </summary>
        /// <param name="email">email</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">Không có gì xảy ra !</exception>
        /// <response Code="404">Không tìm thấy</response>
        [HttpGet("logout/{email}")]
        public async Task<ActionResult<bool>> LogoutCustomer(string email)
        {
            var data = await _loginSvc.Logout(email);
            if (data == false)
            {
                return NotFound();
            }
            return data;
        }
    }
}
