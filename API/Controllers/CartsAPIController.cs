using Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/carts")]
    [ApiController]
    public class CartsAPIController : ControllerBase
    {
        private readonly ILookupSvc<string, Cart> _lookupsvc;
        public CartsAPIController(ILookupSvc<string, Cart> lookupsvc)
        {
            _lookupsvc = lookupsvc;
        }

        /// <summary>
        /// Lấy thông tin id giỏ hàng theo email
        /// </summary>
        /// <param name="email">email</param>
        /// <response Code="404">Không tìm thấy</response>
        /// <returns>Thông tin giỏ hàng</returns>
        [HttpGet("email/{email}")]
        public async Task<ActionResult<Cart>> GetCart(string email)
        {
            var data = await _lookupsvc.GetDataByKey(email);
            if (data == null)
            {
                return NotFound();
            }
            return data;
        }
    }
}
