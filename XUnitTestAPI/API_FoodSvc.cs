using API.Services.Implement;
using DTO;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API;
using API.Services.Interfaces;

namespace XUnitTestAPI
{
    public class API_FoodSvc
    {
        private CustomerSvc _customerSvc;
        public API_FoodSvc(CustomerSvc customerSvc)
        {
            _customerSvc = customerSvc;
        }

        [Fact(DisplayName = "CustomerSvc - Add")]
        public async void AddCustomer()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps30117@fpt.edu.vn";
            customer.PassWord = "Giahao1";
            Assert.NotNull(await _customerSvc.AddNewData(customer));
        }

        [Fact(DisplayName = "CustomerSvc - Edit")]
        public async void EditCustomer()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps30117@fpt.edu.vn";
            customer.PassWord = "Giahao200";
            Assert.NotNull(await _customerSvc.EditData(customer));
        }

        [Fact(DisplayName = "CustomerSvc - GetList")]
        public async void GetCustomers()
        {
            var customers = await _customerSvc.ReadDatas();
            Assert.NotNull(customers);
        }

        [InlineData("haotgps30117@fpt.edu.vn")]
        public async void GetCustomerByEmail(string email)
        {
            Assert.NotNull(await _customerSvc.GetDataByKey(email));
        }

        [InlineData("haotgps30117@fpt.edu.vn")]
        public async void DeleteCustomers(string email)
        {
            string ex = $"Xóa {email} thành công !";
            Assert.Equal(ex, await _customerSvc.DeleteData(email));
        }
    }
}
