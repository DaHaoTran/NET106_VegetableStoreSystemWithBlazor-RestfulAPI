using API.Context;
using Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implement
{
    public class CustomerInformationSvc : ILookupSvc<int, CustomerInformation>, IAddable<CustomerInformation>, IReadable<CustomerInformation>, IDeletable<int, CustomerInformation>, IEditable<CustomerInformation>
    {
        private FoodShopDBContext _dbContext;
        public CustomerInformationSvc(FoodShopDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CustomerInformation> AddNewData(CustomerInformation entity)
        {
            try
            {
                await _dbContext.customerInformation.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return true;
            } catch
            {
                return false;
            }
        }

        public async Task<string> DeleteData(int key)
        {
            try
            {
                var find = await GetDataByKey(key);
                _dbContext.customerInformation.Remove(find);
                await _dbContext.SaveChangesAsync();
                return true;
            } catch
            {
                return false;
            }
        }

        public Task<CustomerInformation> EditData(CustomerInformation entity)
        {
            throw new NotImplementedException();
        }

        public Task<CustomerInformation> GetDataByKey(int key)
        {
            return _dbContext.customerInformation.Where(x => x.CInforId == key).FirstOrDefaultAsync()!;
        }

        public async Task<IEnumerable<CustomerInformation>> ReadDatas()
        {
            return await _dbContext.customerInformation.ToListAsync();
        }
    }
}
