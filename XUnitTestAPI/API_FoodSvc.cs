using API.Services.Implement;
using DTO;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTestAPI
{
    public class API_FoodSvc
    {
        private CustomerSvc _customerSvc;
        private readonly FastFoodDBContext _dbContext;
        public API_FoodSvc(FastFoodDBContext dBContext)
        {
            _dbContext = dBContext;
            if (_customerSvc == null) _customerSvc = new CustomerSvc(_dbContext);
        }

        public async void AddCustomer()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps30117@fpt.edu.vn";
            customer.PassWord = "Giahao1";
            await _customerSvc.AddNewData(customer);
        }

        public async void EditCustomer()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps30117@fpt.edu.vn";
            customer.PassWord = "Giahao200";
            await _customerSvc.EditData(customer);
        }

        public async void GetCustomers()
        {

        }
    }
}
