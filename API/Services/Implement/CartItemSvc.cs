using Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DTO;

namespace API.Services.Implement
{
    public class CartItemSvc : IAddable<CartItem>, IDeletable<int, CartItem>, IEditable<CartItem>, ILookupMoreSvc<int, CartItem>
    {
        private readonly FastFoodDBContext _dbContext;
        public CartItemSvc(FastFoodDBContext dbContext)
        {
            _dbContext = dbContext; 
        }

        private int NewId()
        {
            int id = _dbContext.cartItems.Count() + 1;
            int count = 0;
            while (_dbContext.cartItems.Any(x => x.ItemId == id))
            {
                count++;
                id += count;
            }
            return id;
        }

        public async Task<CartItem> AddNewData(CartItem entity)
        {
            entity.ItemId = NewId();
            await _dbContext.cartItems.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<string> DeleteData(int key)
        {
            var find = await _dbContext.cartItems.Where(x => x.ItemId == key).FirstOrDefaultAsync();
            if(find == default)
            {
                return "Không tìm thấy";
            }
            return $"Xóa {key} thành công !";
        }

        public async Task<CartItem> EditData(CartItem entity)
        {
            var find = await _dbContext.cartItems.Where(x => x.ItemId == entity.ItemId).FirstOrDefaultAsync(); 
            if(find == default)
            {
                return null;
            }
            find.Quantity = entity.Quantity;
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<CartItem>> GetListByKey(int key)
        {
            var find = await _dbContext.cartItems.Where(x => x.CartId == key).ToListAsync();
            if(find.Count == 0)
            {
                return null;
            }
            return find;
        }
    }
}
