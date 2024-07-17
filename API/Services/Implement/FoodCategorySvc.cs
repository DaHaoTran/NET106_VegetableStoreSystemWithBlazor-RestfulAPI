using API.Context;
using Models;
using API.Services.Implement;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DTO;

namespace API.Services.Implement
{
    public class FoodCategorySvc : ILookupSvc<Guid, FoodCategory>, ILookupMoreSvc<string, FoodCategory>, IAddable<FoodCategory>, IEditable<FoodCategory>, IDeletable<Guid, FoodCategory>, IReadable<FoodCategory>
    {
        private readonly FastFoodDBContext _dbContext;
        public FoodCategorySvc(FastFoodDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<FoodCategory> AddNewData(FoodCategory entity)
        {
            var find = await _dbContext.foodCategories.Where(x => x.CategoryName.Contains(entity.CategoryName, StringComparison.OrdinalIgnoreCase)).FirstOrDefaultAsync();
            if (find != default)
            {
                return null;
            }
            entity.FCategoryCode = new Guid();
            await _dbContext.foodCategories.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<string> DeleteData(Guid key)
        {
            var find = await _dbContext.foodCategories.Where(x => x.FCategoryCode == key).FirstOrDefaultAsync();
            if(find == default)
            {
                return "Không tìm thấy";
            }
            _dbContext.foodCategories.Remove(find);
            await _dbContext.SaveChangesAsync();
            return $"Xóa {key} thành công !";
        }

        public async Task<FoodCategory> EditData(FoodCategory entity)
        {
            var find = await _dbContext.foodCategories.Where(x => x.FCategoryCode == entity.FCategoryCode).FirstOrDefaultAsync();
            if(find == default)
            {
                return null;
            }
            find.CategoryName = entity.CategoryName;
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<FoodCategory>> GetListByKey(string key)
        {
            var find = await _dbContext.foodCategories.Where(x => x.CategoryName.Contains(key, StringComparison.OrdinalIgnoreCase)).ToListAsync();
            if(find == default)
            {
                return null;
            } 
            return find;
        }

        public async Task<FoodCategory> GetDataByKey(Guid key)
        {
            var find = await _dbContext.foodCategories.Where(x => x.FCategoryCode == key).FirstOrDefaultAsync()!;
            if (find == default)
            {
                return null;
            }
            return find;
        }

        public async Task<IEnumerable<FoodCategory>> ReadDatas()
        {
            return await _dbContext.foodCategories.ToListAsync();
        }
    }
}
