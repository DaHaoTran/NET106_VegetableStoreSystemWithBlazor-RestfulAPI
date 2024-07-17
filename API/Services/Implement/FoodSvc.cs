using API.Context;
using Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DTO;

namespace API.Services.Implement
{
    public class FoodSvc : ILookupMoreSvc<string, Food>, IAddable<Food>, IEditable<Food>, IDeletable<Guid, Food>, IReadable<Food>
    {
        private readonly FastFoodDBContext _dbContext;
        public FoodSvc(FastFoodDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Thêm một thức ăn mới
        /// </summary>
        /// <remarks>
        /// Lưu ý:
        /// Hãy có một foodCategory trong foodCategories (table database) trước khi thực hiện thêm mới này
        /// Hãy có một admin trong admins (table database) trước khi thực hiện thêm mới này
        /// </remarks>
        /// <example>
        /// {
        ///     "foodName": "Chicken Fried",
        ///     "currentPrice": "25000",
        ///     "left": 100,
        ///     "image": "0394837463.png",
        ///     "fCategoryCode: "..." (mã phân loại),
        ///     "adminCode": "..." (mã quản trị)
        /// }
        /// </example>
        /// <returns></returns>
        public async Task<Food> AddNewData(Food entity)
        {
            var data = await _dbContext.foods.Where(x => x.FoodName == entity.FoodName).FirstOrDefaultAsync();
            if(data != default)
            {
                return null;
            }
            Task t = Task.Run(() =>
            {
                entity.FoodCode = new Guid();
                entity.PreviousPrice = entity.CurrentPrice;
            });
            t.Wait();
            await _dbContext.foods.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Xóa một thức ăn
        /// </summary>
        /// <param name="key">foodCode</param>
        /// <returns></returns>
        public async Task<string> DeleteData(Guid key)
        {
            var find = await _dbContext.foods.Where(x => x.FoodCode == key).FirstOrDefaultAsync();
            if(find == default)
            {
                return "Không tìm thấy";
            }
            _dbContext.foods.Remove(find);
            await _dbContext.SaveChangesAsync();
            return $"Xóa {key} thành công !";
        }

        /// <summary>
        /// Chỉnh sửa một thức ăn theo foodCode
        /// </summary>
        /// <returns>Thức ăn đã chỉnh sửa</returns>
        public async Task<Food> EditData(Food entity)
        {
            var find = await _dbContext.foods.Where(x => x.FoodCode == entity.FoodCode).FirstOrDefaultAsync();
            if(find == default)
            {
                return null;
            }
            Task t = Task.Run(() =>
            {
                find.PreviousPrice = find.CurrentPrice;
                find.FoodName = entity.FoodName;
                find.CurrentPrice = entity.CurrentPrice;
                find.Left = entity.Left;
                find.Sold = entity.Sold;
                find.Image = entity.Image;
                find.FCategoryCode = entity.FCategoryCode;
                find.AdminCode = entity.AdminCode;
            });
            t.Wait();
            await _dbContext.SaveChangesAsync();
            return find;
        }

        /// <summary>
        /// Lấy thông tin thức ăn theo foodName
        /// </summary>
        /// <param name="key">foodName</param>
        /// <returns>Thông tin thức ăn</returns>
        public async Task<IEnumerable<Food>> GetListByKey(string key)
        {
            var data = await _dbContext.foods.Where(x => x.FoodName.Contains(key, StringComparison.OrdinalIgnoreCase)).ToListAsync();
            return data;
        }

        /// <summary>
        /// Lấy danh sách thức ăn
        /// </summary>
        /// <returns>Danh sách thức ăn</returns>
        public async Task<IEnumerable<Food>> ReadDatas()
        {
            return await _dbContext.foods.ToListAsync();
        }
    }
}
