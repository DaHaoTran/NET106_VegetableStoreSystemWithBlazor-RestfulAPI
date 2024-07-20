using API.Context;
using Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DTO;

namespace API.Services.Implement
{
    public class OrderSvc : IAddable<Order>, IDeletable<Guid, Order>, ILookupMoreSvc<string, Order>, IReadable<Order>
    {
        private readonly FastFoodDBContext _dbContext;
        public OrderSvc(FastFoodDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Thêm một đơn hàng mới
        /// </summary>
        /// <remarks>
        /// Lưu ý:
        /// Hãy có một customerInformation trong customerInformations (table database) trước khi thực hiện thêm mới này
        /// Hãy có một customer trong customers (table database) trước khi thực hiện thêm mới này
        /// </remarks>
        /// <example>
        /// {
        ///     "state": "Not Delivered",
        ///     "comment": "test data",
        ///     "paymentMethod": "Thanh toán khi nhận hàng",
        ///     "total": "200000",
        ///     "cInforId": "..." (mã địa chỉ khách hàng),
        ///     "customerEmail": "..." (email tài khoản khách hàng)
        /// }
        /// </example>
        /// <returns></returns>
        public async Task<Order> AddNewData(Order entity)
        {
            Task t = Task.Run(() =>
            {
                entity.OrderCode = new Guid();
                entity.OrderDate = DateTime.Now;
            });
            t.Wait();
            await _dbContext.orders.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Xóa một đơn hàng 
        /// </summary>
        /// <param name="key">orderCode</param>
        /// <returns></returns>
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

        /// <summary>
        /// Lấy thông tin đơn hàng qua email
        /// </summary>
        /// <param name="key">email</param>
        /// <returns>Thông tin đơn hàng</returns>
        public async Task<IEnumerable<Order>> GetListByKey(string key)
        {
            var find = await _dbContext.orders.Where(x => x.CustomerEmail == key).ToListAsync();
            return find;
        }
    }
}
