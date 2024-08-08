using Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using DTO;

namespace API.Services.Implement
{
    public class CustomerInformationSvc : ILookupSvc<int, CustomerInformation>, ILookupMoreSvc<string, CustomerInformation>, ILookupSvc<string, CustomerInformation>, IAddable<CustomerInformation>, IReadable<CustomerInformation>, IDeletable<int, CustomerInformation>, IEditable<CustomerInformation>
    {
        private readonly FastFoodDBContext _dbContext;
        public CustomerInformationSvc(FastFoodDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        private int NewId()
        {
            int id = _dbContext.customerInformations.Count() + 1;
            int count = 0;
            while(_dbContext.customerInformations.Any(x => x.CInforId == id))
            {
                count++;
                id += count;
            }
            return id;
        }

        public async Task<CustomerInformation> AddNewData(CustomerInformation entity)
        {
            entity.CInforId = NewId();
            if(_dbContext.customers.Any(x => x.Email == entity.CustomerEmail))
            {
                await _dbContext.customerInformations.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return entity;
            }
            return null;
        }

        public async Task<string> DeleteData(int key)
        {
            var find = await _dbContext.customerInformations.Where(x => x.CInforId == key).FirstOrDefaultAsync();
            if(find == default)
            {
                return "Không tìm thấy";
            }
            _dbContext.customerInformations.Remove(find);
            await _dbContext.SaveChangesAsync();
            return $"Xóa {key} thành công !";
        }

        public async Task<CustomerInformation> EditData(CustomerInformation entity)
        {
            var find = await _dbContext.customerInformations.Where(x => x.CInforId == entity.CInforId).FirstOrDefaultAsync();
            if(find == default)
            {
                return null;
            }
            find.PhoneNumber = entity.PhoneNumber;
            find.CustomerName = entity.CustomerName;
            find.Address = entity.Address;
            await _dbContext.SaveChangesAsync();
            return find;
        }

        public async Task<CustomerInformation> GetDataByKey(int key)
        {
            var find = await _dbContext.customerInformations.Where(x => x.CInforId == key).FirstOrDefaultAsync();
            if (find == default)
            {
                return null;
            }
            return find;
        }

        public async Task<IEnumerable<CustomerInformation>> ReadDatas()
        {
            return await _dbContext.customerInformations.ToListAsync();
        }

        public async Task<IEnumerable<CustomerInformation>> GetListByKey(string key)
        {
            var find = await _dbContext.customerInformations.Where(x => x.CustomerEmail == key).ToListAsync();
            if(find.Count == 0)
            {
                find = await _dbContext.customerInformations.Where(x => x.CustomerName.Contains(key, StringComparison.OrdinalIgnoreCase)).ToListAsync();
                if(find.Count == 0)
                {
                    find = await _dbContext.customerInformations.Where(x => x.Address.Contains(key, StringComparison.OrdinalIgnoreCase)).ToListAsync();
                    if(find.Count == 0)
                    {
                        return null;
                    }
                }
            }
            return find;
        }

        public async Task<CustomerInformation> GetDataByKey(string key)
        {
            var find = await _dbContext.customerInformations.Where(x => x.PhoneNumber == key).FirstOrDefaultAsync();
            if (find == default)
            {
                return null;
            }
            return find;
        }
    }
}
