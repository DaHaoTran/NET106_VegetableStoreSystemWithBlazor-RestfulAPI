using API.Context;
using Models;
using API.Services.Implement;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DTO;

namespace API.Services.Implement
{
    public class CustomerSvc : ILookupSvc<string, Customer>, IAddable<Customer>, IEditable<Customer>, IDeletable<string, Customer>, IReadable<Customer>
    {
        private readonly FastFoodDBContext _dbContext;
        public CustomerSvc(FastFoodDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        private static string CutStringBeforeAt(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input string cannot be null or empty", nameof(input));
            }

            int atIndex = input.IndexOf('@');
            if (atIndex == -1)
            {
                throw new ArgumentException("Input string does not contain '@' character", nameof(input));
            }

            return input.Substring(0, atIndex);
        }

        public async Task<Customer> AddNewData(Customer entity)
        {
            var find = await _dbContext.customers.Where(x => x.Email == entity.Email).FirstOrDefaultAsync();
            if (find != default)
            {
                return null;
            }
            entity.UserName = CutStringBeforeAt(entity.Email);
            entity.PassWord = AuthencationDataSvc.EncryptionPassword(entity.PassWord);
            await _dbContext.customers.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<string> DeleteData(string key)
        {
            var find = await _dbContext.customers.Where(x => x.Email == key).FirstOrDefaultAsync();
            if (find == default)
            {
                return "Không tìm thấy";
            }
            _dbContext.customers.Remove(find);
            await _dbContext.SaveChangesAsync();
            return $"Xóa {key} thành công !";
        }

        public async Task<Customer> EditData(Customer entity)
        {
            var find = await _dbContext.customers.Where(x => x.Email == entity.Email).FirstOrDefaultAsync();
            if(find == default)
            {
                return null;
            }
            find.PassWord = AuthencationDataSvc.EncryptionPassword(entity.PassWord);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<Customer> GetDataByKey(string key)
        {
            var find = await _dbContext.customers.Where(x => x.Email == key).FirstOrDefaultAsync();
            if (find == default)
            {
                return null;
            }
            return find;
        }

        public async Task<IEnumerable<Customer>> ReadDatas()
        {
            return await _dbContext.customers.ToListAsync();
        }
    }
}
