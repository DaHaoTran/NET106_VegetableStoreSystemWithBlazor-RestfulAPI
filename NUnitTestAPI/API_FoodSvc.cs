using API.Services.Implement;
using DTO;
using Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API;

namespace XUnitTestAPI
{
    public class API_FoodSvc
    {
        private Bunit.TestContext testContext;
        private Mock<CustomerSvc> loginSvc;

        [SetUp]
        public void Setup()
        {
            testContext = new Bunit.TestContext();
            loginSvc = new Mock<CustomerSvc>();
        }

        [TearDown]
        public void Teardown()
        {
            testContext.Dispose();
        }
        public async void AddCustomer()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps30117@fpt.edu.vn";
            customer.PassWord = "Giahao1";
            Assert.NotNull(await _customerSvc.AddNewData(customer));
        }

        public async void EditCustomer()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps30117@fpt.edu.vn";
            customer.PassWord = "Giahao200";
            Assert.NotNull(await _customerSvc.EditData(customer));
        }

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
