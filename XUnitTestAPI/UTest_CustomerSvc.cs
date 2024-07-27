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
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Microsoft.AspNetCore.SignalR;
using Castle.Core.Resource;

namespace XUnitTestAPI
{
    public class UTest_CustomerSvc
    {
        private CustomerSvc _customerSvc;
        private FastFoodDBContext _dbContext;
        private DbContextOptions<FastFoodDBContext> _options;

        public UTest_CustomerSvc()
        {
            _options = new DbContextOptionsBuilder<FastFoodDBContext>()
            .UseInMemoryDatabase(databaseName: "NET106_ASM_FastFood")
            .Options;
            _dbContext = new FastFoodDBContext(_options);
            if (_customerSvc == null) _customerSvc = new CustomerSvc(_dbContext);
            CreateNewCustomer();
        }

        //Add
        [Fact(DisplayName = "CustomerSvc - Add failed with exist email")]
        public async void AddCustomerWithExistsEmail()
        {
            List<Customer> list = new List<Customer>()
            {
                new Customer { Email = "haotgps30117@fpt.edu.vn", PassWord = "Giahao1"},
                new Customer { Email = "haotgps30117@fpt.edu.vn", PassWord = "Giahao2"}
            };
            int count = 0;
            foreach(var customer in list)
            {
                count++;
                if (count == 2)
                {
                    Assert.Null(await _customerSvc.AddNewData(customer));
                }
                await _customerSvc.AddNewData(customer);
            }
        }

        [Fact(DisplayName = "CustomerSvc - Add failed with null password")]
        public async void AddCustomerWithNullPassword()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps30117@fpt.edu.vn";
            Assert.Null(await _customerSvc.AddNewData(customer));
        }

        [Fact(DisplayName = "CustomerSvc - Add sucessfully")]
        public async void AddCustomerSucessfully()
        {
            Customer customer = new Customer();
            customer.Email = "bachdb30284@fpt.edu.vn";
            customer.PassWord = "Giahao1";
            Assert.Equal(customer, await _customerSvc.AddNewData(customer));
        }

        //Edit
        [Fact(DisplayName = "CustomerSvc - Edit failed with null email")]
        public async void EditCustomerWithNullEmail()
        {
            Customer customer = new Customer();
            customer.PassWord = "Giahao200";
            Assert.Null(await _customerSvc.EditData(customer));
        }

        [Fact(DisplayName = "CustomerSvc - Edit failed with null password")]
        public async void EditCustomerWithNullPassword()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps30117@fpt.edu.vn";
            customer.PassWord = null;
            Assert.Null(await _customerSvc.EditData(customer));
        }

        [Fact(DisplayName = "CustomerSvc - Edit failed with not found email")]
        public async void EditCustomerWithNotFoundEmail()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps20118@fpt.edu.vn";
            customer.PassWord = "giahao200";
            Assert.Null(await _customerSvc.EditData(customer));
        }

        [Fact(DisplayName = "CustomerSvc - Edit sucessfully")]
        public async void EditCustomerSucessfully()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps30117@fpt.edu.vn";
            customer.PassWord = "Giahao200";
            //_editsvc.Setup(x => x.EditData(customer)).ReturnsAsync(customer);
            //_editsvc.Verify(x => x.EditData(customer), Times.Once);
            Assert.Equal(customer, await _customerSvc.EditData(customer));
        }

        //GetList
        [Fact(DisplayName = "CustomerSvc - Get list")]
        public async void GetCustomers()
        {
            //_readsvc.Setup(x => x.ReadDatas()).ReturnsAsync(customers);
            //_readsvc.Verify(x => x.ReadDatas(), Times.Once);
            Assert.NotNull(await _customerSvc.ReadDatas());
        }

        //GetByEmail
        [Xunit.Theory(DisplayName = "CustomerSvc - Get by email with null email")]
        [InlineData(null)]
        public async void GetCustomerByEmailWithNullEmail(string email)
        {
            var customer = await _customerSvc.GetDataByKey(email);
            Assert.Null(customer);
        }

        [Xunit.Theory(DisplayName = "CustomerSvc - Get by email with not found email")]
        [InlineData("haotgps30118@fpt.edu.vn")]
        public async void GetCustomerByEmailWithNotFoundEmail(string email)
        {
            var customer = await _customerSvc.GetDataByKey(email);
            Assert.Null(customer);
        }

        [Xunit.Theory(DisplayName = "CustomerSvc - Get by email")]
        [InlineData("haotgps30117@fpt.edu.vn")]
        public async void GetCustomerByEmail(string email)
        {
            //_lookupsvc.Setup(x => x.GetDataByKey(email)).ReturnsAsync(customer);
            //_lookupsvc.Verify(x => x.GetDataByKey(email), Times.Once);
            var customer = await _customerSvc.GetDataByKey(email);
            Assert.Equal(email, customer.Email);
        }

        //Delete
        [Xunit.Theory(DisplayName = "CustomerSvc - Delete with not found email")]
        [InlineData("haotgps20117@fpt.edu.vn")]
        public async void DeleteCustomerWithNotFoundEmail(string email)
        {
            string ex = "Không tìm thấy";
            Assert.Equal(ex, await _customerSvc.DeleteData(email));
        }

        [Xunit.Theory(DisplayName = "CustomerSvc - Delete with null email")]
        [InlineData(null)]
        public async void DeleteCustomerWithNotNullEmail(string email)
        {
            string ex = "Không tìm thấy";
            Assert.Equal(ex, await _customerSvc.DeleteData(email));
        }

        [Xunit.Theory(DisplayName = "CustomerSvc - Delete Sucessfully")]
        [InlineData("haotgps30117@fpt.edu.vn")]
        public async void DeleteCustomer(string email)
        {
            string ex = $"Xóa {email} thành công !";
            //_deletesvc.Setup(x => x.DeleteData(email)).ReturnsAsync(ex);
            //_deletesvc.Verify(x => x.DeleteData(email), Times.Once);
            Assert.Equal(ex, await _customerSvc.DeleteData(email));
        }

        //Others
        [Fact(Skip = "Function")]
        private async void CreateNewCustomer()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps30117@fpt.edu.vn";
            customer.PassWord = "Giahao1";
            await _customerSvc.AddNewData(customer);
        }
    }
}
