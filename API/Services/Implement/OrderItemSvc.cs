using Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using DTO;

namespace API.Services.Implement
{
    public class OrderItemSvc : IAddable<List<OrderItem>>, ILookupMoreSvc<Guid, OrderItem>
    {
        private readonly FastFoodDBContext _dbContext;
        public OrderItemSvc(FastFoodDBContext dbContext) {
            _dbContext = dbContext;
        }

        private int NewId()
        {
            int id = _dbContext.orderItems.Count() + 1;
            int count = 0;
            while (_dbContext.guests.Any(x => x.GuesId == id))
            {
                count++;
                id += count;
            }
            return id;
        }

        public async Task<List<OrderItem>> AddNewData(List<OrderItem> entity)
        {
            for(int i = 0; i < entity.Count; i++)
            {
                entity[i].ItemId = NewId();
            }
            await _dbContext.orderItems.AddRangeAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<OrderItem>> GetListByKey(Guid key)
        {
            var find = await _dbContext.orderItems.Where(x => x.OrderCode == key).ToListAsync();
            if(find.Count == 0)
            {
                return null;
            }
            return find;
        }
    }
}
