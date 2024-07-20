using API.Context;
using API.Services.Interfaces;
using DTO;
using Models;
using API.Services.Implement;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implement
{
    public class CustomerLoginSvc : ILoginSvc<Customer>
    {
        private readonly FastFoodDBContext _dbContext;

        public CustomerLoginSvc(FastFoodDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Login(Customer entity)
        {
            var customer = await _dbContext.customers.Where(x => x.Email == entity.Email).FirstOrDefaultAsync();
            if (customer != default)
            {
                if (customer.PassWord == AuthencationDataSvc.EncryptionPassword(entity.PassWord))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public Task<bool> Logout(string email)
        {
            throw new NotImplementedException();
        }
    }
}
