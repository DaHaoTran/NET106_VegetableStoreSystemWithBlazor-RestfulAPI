using API.Services.Interfaces;
using DTO;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections;

namespace API.Services.Implement
{
    public class ComboSvc : IAddable<Combo>, IEditable<Combo>, IDeletable<Guid, Combo>, IReadable<Combo>, ILookupSvc<Guid, Combo>, ILookupMoreSvc<string, Combo>
    {
        private FastFoodDBContext _dbContext;
        public ComboSvc(FastFoodDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Thêm một combo mới
        /// </summary>
        /// <example>
        /// {
        ///     "comboName": "Family Combo",
        ///     "currentPrice: "100000",
        ///     "image": "9304938273.jpg",
        ///     "expDate: "30/08/2024 12:48:32 CH"
        /// }
        /// </example>
        /// <returns></returns>
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

        /// <summary>
        /// Xóa một combo
        /// </summary>
        /// <param name="key">comboCode</param>
        /// <returns></returns>
        public async Task<string> DeleteData(Guid key)
        {
            var find = await _dbContext.combos.Where(x => x.ComboCode == key).FirstOrDefaultAsync();
            if(find == default)
            {
                return "Không tìm thấy";
            }
            _dbContext.combos.Remove(find);
            await _dbContext.SaveChangesAsync();
            return $"Xóa {key} thành công";
        }

        /// <summary>
        /// Sửa một combo được chọn theo comboCode
        /// </summary>
        /// <returns>Combo đã chỉnh sửa</returns>
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

        /// <summary>
        /// Lấy thông tin combo theo comboCode
        /// </summary>
        /// <param name="key">comboCode</param>
        /// <returns>Thông tin combo</returns>
        public async Task<Combo> GetDataByKey(Guid key)
        {
            var find = await _dbContext.combos.Where(x => x.ComboCode == key).FirstOrDefaultAsync();
            if (find == default)
            {
                return null;
            }
            return find;
        }

        /// <summary>
        /// Lấy thông tin combo theo comboName
        /// </summary>
        /// <param name="key">comboName</param>
        /// <returns>Thông tin combo</returns>
        public async Task<IEnumerable<Combo>> GetListByKey(string key)
        {
            var find = await _dbContext.combos.Where(x => x.ComboName.Contains(key, StringComparison.OrdinalIgnoreCase)).ToListAsync();
            return find;
        }

        /// <summary>
        /// Lấy danh sách combo
        /// </summary>
        /// <returns>Danh sách combo</returns>
        public async Task<IEnumerable<Combo>> ReadDatas()
        {
            return await _dbContext.combos.ToListAsync();
        }
    }
}
