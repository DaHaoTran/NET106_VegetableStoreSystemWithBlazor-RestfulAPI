using API.Context;
using Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DTO;

namespace API.Services.Implement
{
    public class FoodSvc : ILookupSvc<string, Food>, IAddable<Food>, IEditable<Food>, IDeletable<Guid, Food>, IReadable<Food>
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

        public async Task<string> DeleteData(Guid key)
        {
            try
            {
                var find = await GetDataByKey(key);
                _dbContext.foods.Remove(find);
                await _dbContext.SaveChangesAsync();
                return true;
            } catch
            {
                return false;
            }
        }

        public async Task<Food> EditData(Food entity)
        {
            try
            {
                var find = await GetDataByKey(entity.FoodCode);
                Task task = Task.Run(async () =>
                {
                    var admin = await _dbContext.admins.Where(x => x.Email == entity.AdminCode).FirstOrDefaultAsync();
                    find.AdminCode = admin!.AdminCode;
                    find.FoodName = entity.FoodName;
                    find.PreviousPrice = find.CurrentPrice;
                    find.CurrentPrice = entity.CurrentPrice;
                    if(find.Sold == null)
                    {
                        find.Sold = 0;
                    }
                    else
                    {
                        find.Sold = entity.Sold;
                    }
                    find.Left = entity.Left;
                    find.Image = entity.Image;
                    find.FCategoryCode = await _dbContext.foodCategory.Where(x => x.CategoryName == entity.FCategoryCode).Select(x => x.FCategoryCode).FirstOrDefaultAsync()!;
                    find.FTypeCode = await _dbContext.foodType.Where(x => x.TypeName == entity.FTypeCode).Select(x => x.FTypeCode).FirstOrDefaultAsync()!;
                });
                task.Wait();
                await _dbContext.SaveChangesAsync();
                return true;
            } catch
            {
                return false;
            }
        }

        public Task<Food> GetDataByKey(string key)
        {
            return _dbContext.foods.Where(x => x.FoodCode == key).FirstOrDefaultAsync()!;
        }

        public async Task<IEnumerable<Food>> ReadDatas()
        {
            return await _dbContext.foods.ToListAsync();
        }
    }
}
