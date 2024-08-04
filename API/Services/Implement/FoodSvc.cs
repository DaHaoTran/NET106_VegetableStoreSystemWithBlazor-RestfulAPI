using Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DTO;

namespace API.Services.Implement
{
    public class FoodSvc : ILookupMoreSvc<string, Food>, ILookupSvc<Guid, Food>, IAddable<Food>, IEditable<Food>, IDeletable<Guid, Food>, IReadable<Food>
    {
        private readonly FastFoodDBContext _dbContext;
        public FoodSvc(FastFoodDBContext dbContext)
        {
            _dbContext = dbContext;
        }

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
            var find = await _dbContext.foods.Where(x => x.FoodCode == key).FirstOrDefaultAsync();
            if(find == default)
            {
                return "Không tìm thấy";
            }
            _dbContext.foods.Remove(find);
            await _dbContext.SaveChangesAsync();
            return $"Xóa {key} thành công !";
        }

        public async Task<Food> EditData(Food entity)
        {
            var find = await _dbContext.foods.Where(x => x.FoodCode == entity.FoodCode).FirstOrDefaultAsync();
            if(find == default)
            {
                return null;
            }
            var find2 = await _dbContext.foods.Where(x => x.FoodName == entity.FoodName && x.FoodCode != find.FoodCode).FirstOrDefaultAsync();
            if(find2 != default)
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

        public async Task<Food> GetDataByKey(Guid key)
        {
            var find = await _dbContext.foods.Where(x => x.FoodCode ==  key).FirstOrDefaultAsync(); 
            if(find == default)
            {
                return null;
            }
            return find;
        }

        public async Task<IEnumerable<Food>> GetListByKey(string key)
        {
            var find = await _dbContext.foods.Where(x => x.FoodName.Contains(key, StringComparison.OrdinalIgnoreCase)).ToListAsync();
            if(find.Count == 0)
            {
                return null;
            }
            return find;
        }

        public async Task<IEnumerable<Food>> ReadDatas()
        {
            return await _dbContext.foods.ToListAsync();
        }
    }
}
