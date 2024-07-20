using API.Services.Interfaces;
using DTO;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections;

namespace API.Services.Implement
{
    public class ComboSvc : IAddable<Combo>, IEditable<Combo>, IDeletable<Guid, Combo>, IReadable<Combo>, ILookupSvc<Guid, Combo>, ILookupMoreSvc<string, Combo>
    {
        private readonly FastFoodDBContext _dbContext;
        public ComboSvc(FastFoodDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Combo> AddNewData(Combo entity)
        {
            var find = await _dbContext.combos.Where(x => x.ComboName.Contains(entity.ComboName, StringComparison.OrdinalIgnoreCase)).FirstOrDefaultAsync();
            if(find != default)
            {
                return null;
            }

            Task t = Task.Run(() =>
            {
                entity.ComboCode = new Guid();
                entity.PreviousPrice = entity.CurrentPrice;
                entity.ApplyDate = DateTime.Now;
            });
            t.Wait();
            await _dbContext.combos.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<string> DeleteData(Guid key)
        {
            var find = await _dbContext.combos.Where(x => x.ComboCode == key).FirstOrDefaultAsync();
            if(find == default)
            {
                return "Không tìm thấy";
            }
            _dbContext.combos.Remove(find);
            await _dbContext.SaveChangesAsync();
            return $"Xóa {key} thành công !";
        }

        public async Task<Combo> EditData(Combo entity)
        {
            var find = await _dbContext.combos.Where(x => x.ComboCode == entity.ComboCode).FirstOrDefaultAsync();
            if(find == default)
            {
                return null;
            }
            Task t = Task.Run(() =>
            {
                find.PreviousPrice = find.CurrentPrice;
                find.ComboName = entity.ComboName;
                find.CurrentPrice = entity.CurrentPrice;
                find.Image = entity.Image;
                find.ExpDate = entity.ExpDate;
            });
            t.Wait();
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<Combo> GetDataByKey(Guid key)
        {
            var find = await _dbContext.combos.Where(x => x.ComboCode == key).FirstOrDefaultAsync();
            if (find == default)
            {
                return null;
            }
            return find;
        }

        public async Task<IEnumerable<Combo>> GetListByKey(string key)
        {
            var find = await _dbContext.combos.Where(x => x.ComboName.Contains(key, StringComparison.OrdinalIgnoreCase)).ToListAsync();
            return find;
        }

        public async Task<IEnumerable<Combo>> ReadDatas()
        {
            return await _dbContext.combos.ToListAsync();
        }
    }
}
