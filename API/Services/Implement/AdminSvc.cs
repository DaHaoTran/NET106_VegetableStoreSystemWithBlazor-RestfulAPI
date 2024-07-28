using API.Services.Interfaces;
using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models;

namespace API.Services.Implement
{
    public class AdminSvc : IReadable<Admin>, IAddable<Admin>, IEditable<Admin>, IDeletable<Guid, Admin>, ILookupSvc<Guid, Admin>, ILookupSvc<string, Admin>
    {
        private readonly FastFoodDBContext _dbContext;
        public AdminSvc(FastFoodDBContext dBContext) 
        {
            _dbContext = dBContext;
        }

        public async Task<Admin> AddNewData(Admin entity)
        {
            var exist = await _dbContext.admins.Where(x => x.Email == entity.Email).FirstOrDefaultAsync();
            if (exist != default)
            {
                return null;
            } 
                
            Task setT = Task.Run(() =>
                {
                    entity.AdminCode = new Guid();
                    entity.CreatedDate = DateTime.Now;
                    entity.IsOnl = false;
                    entity.Password = AuthencationDataSvc.EncryptionPassword(entity.Password);
                });
            setT.Wait();
            await _dbContext.admins.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<string> DeleteData(Guid key)
        {
            var find = await _dbContext.admins.Where(x => x.AdminCode == key).FirstOrDefaultAsync();
            if(find == default)
            {
                return "Không tìm thấy";
            }
            if (find.IsOnl == true)
            {
                return "Xóa thất bại, thuộc tính IsOnl đang là true !";
            }
            else
            {
                _dbContext.admins.Remove(find);
                await _dbContext.SaveChangesAsync();
                return $"Xóa {key} thành công !";
            }
        }

        public async Task<Admin> EditData(Admin entity)
        {
            var find = await _dbContext.admins.Where(x => x.Email == entity.Email).FirstOrDefaultAsync();
            if(find == default)
            {
                return null;
            }
            if (find.IsOnl == true)
            {
                return null;
            }
            Task task = Task.Run(() =>
            {
                find.Email = entity.Email;
                find.Level = entity.Level;
                find.Password = AuthencationDataSvc.EncryptionPassword(entity.Password);
            });
            task.Wait();
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<Admin> GetDataByKey(Guid key)
        {
            var find = await _dbContext.admins.Where(x => x.AdminCode == key).FirstOrDefaultAsync();
            if(find == default)
            {
                return null;
            }
            return find;
        }

        public async Task<Admin> GetDataByKey(string key)
        {
            var find = await _dbContext.admins.Where(x => x.Email == key).FirstOrDefaultAsync();
            if (find == default)
            {
                return null;
            }
            return find;
        }

        public async Task<IEnumerable<Admin>> ReadDatas()
        {
            return await _dbContext.admins.ToListAsync();
        }
    }
}
