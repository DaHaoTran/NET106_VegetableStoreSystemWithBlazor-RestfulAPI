using Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using DTO;

namespace API.Services.Implement
{
    public class CartSvc : ILookupSvc<string, Cart>
    {
        private readonly FastFoodDBContext _dbContext;
        public CartSvc(FastFoodDBContext dbContext)
        {
            _dbContext = dbContext; 
        }

        public async Task<Cart> GetDataByKey(string key)
        {
            var find = await _dbContext.carts.Where(x => x.CustomerEmail == key).FirstOrDefaultAsync();
            if(find == default)
            {
                return null;
            }
            return find;
        }
    }
}
