using API.Services.Interfaces;
using DTO;
using Microsoft.EntityFrameworkCore;
using Models;

namespace API.Services.Implement
{
    public class ComboDetailSvc : IAddable<ComboDetail>, IEditable<ComboDetail>, IDeletable<int, ComboDetail>, ILookupMoreSvc<Guid, ComboDetail>, IReadable<ComboDetail>, ILookupSvc<int,  ComboDetail>
    {
        private readonly FastFoodDBContext _dbContext;
        public ComboDetailSvc(FastFoodDBContext dbContext) 
        { 
            _dbContext = dbContext; 
        }

        private int NewId()
        {
            int id = _dbContext.comboDetails.Count() + 1;
            int count = 0;
            while (_dbContext.comboDetails.Any(x => x.Id == id))
            {
                count++;
                id += count;
            }
            return id;
        }

        public async Task<ComboDetail> AddNewData(ComboDetail entity)
        {
            var find = await _dbContext.comboDetails.Where(x => x.ComboCode == entity.ComboCode).ToListAsync();
            if(find != null)
            {
                var exist = find.Where(x => x.FoodCode == entity.FoodCode).FirstOrDefault();
                if (exist != default) 
                {
                    return null;
                }
            }
            entity.Id = NewId();
            await _dbContext.comboDetails.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<ComboDetail> EditData(ComboDetail entity)
        {
            var find = await _dbContext.comboDetails.Where(x => x.Id == entity.Id).FirstOrDefaultAsync();
            if (find == default)
            {
                return null;
            }
            Task t = Task.Run(() =>
            {
                find.ComboCode = entity.ComboCode;
                find.FoodCode = entity.FoodCode;
            });
            t.Wait();
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<string> DeleteData(int key)
        {
            var find = await _dbContext.comboDetails.Where(x => x.Id == key).FirstOrDefaultAsync();
            if(find == default)
            {
                return "Không tìm thấy";
            }
            _dbContext.comboDetails.Remove(find);
            await _dbContext.SaveChangesAsync();
            return $"Xóa {key} thành công !";
        }

        public async Task<IEnumerable<ComboDetail>> GetListByKey(Guid key)
        {
            var find = await _dbContext.comboDetails.Where(x => x.ComboCode == key).ToListAsync();
            if (find.Count == 0)
            {
                return null;
            }
            return find;
        }

        public async Task<IEnumerable<ComboDetail>> ReadDatas()
        {
            return await _dbContext.comboDetails.ToListAsync();
        }

        public async Task<ComboDetail> GetDataByKey(int key)
        {
            var find = await _dbContext.comboDetails.Where(x => x.Id == key).FirstOrDefaultAsync();
            if(find == default)
            {
                return null;
            }
            return find;
        }
    }
}
