using Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DTO;

namespace API.Services.Implement
{
    public class OrderSvc : IAddable<Order>, IDeletable<Guid, Order>, ILookupMoreSvc<string, Order>, IReadable<Order>, IEditable<Order>
    {
        private readonly FastFoodDBContext _dbContext;
        public OrderSvc(FastFoodDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Order> AddNewData(Order entity)
        {
            Task t = Task.Run(() =>
            {
                entity.OrderCode = new Guid();
                entity.OrderDate = DateTime.Now;
                entity.State = "Not delivered";
            });
            t.Wait();
            await _dbContext.orders.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<string> DeleteData(Guid key)
        {
            var find = await _dbContext.orders.Where(x => x.OrderCode == key).FirstOrDefaultAsync();
            if(find == default)
            {
                return "Không tìm thấy";
            }
            _dbContext.orders.Remove(find);
            await _dbContext.SaveChangesAsync();
            return $"Xóa {key} thành công !";
        }

        public async Task<Order> EditData(Order entity)
        {
            var find = await _dbContext.orders.Where(x => x.OrderCode == entity.OrderCode).FirstOrDefaultAsync();
            if(find == default)
            {
                return null;
            }
            find.State = entity.State;
            if(entity.State == "Delivered")
            {
                entity.DeliveryDate = DateTime.Now;
            }
            await _dbContext.SaveChangesAsync();
            return find;
        }

        public async Task<IEnumerable<Order>> GetListByKey(string key)
        {
            var find = await _dbContext.orders.Where(x => x.CustomerEmail == key).ToListAsync();
            if(find.Count == 0)
            {
                return null;
            }
            return find;
        }
       
        public async Task<IEnumerable<Order>> ReadDatas()
        {
            return await _dbContext.orders.ToListAsync();
        }
    }
}
