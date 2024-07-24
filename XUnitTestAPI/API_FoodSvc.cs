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

namespace XUnitTestAPI
{
    public class API_FoodSvc
    {
        private CustomerSvc _customerSvc;
        private FastFoodDBContext _dbContext;
        private DbContextOptions<FastFoodDBContext> _options;

        public API_FoodSvc()
        {
            _options = new DbContextOptionsBuilder<FastFoodDBContext>()
            .UseInMemoryDatabase(databaseName: "NET106_ASM_FastFood")
            .Options;
            _dbContext = new FastFoodDBContext(_options);
            if (_customerSvc == null) _customerSvc = new CustomerSvc(_dbContext);
        }

        [Fact(DisplayName = "CustomerSvc - Add sucessfully")]
        public async void AddCustomerSucessfully()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps30117@fpt.edu.vn";
            customer.PassWord = "Giahao1";
            //_addsvc.Setup(x => x.AddNewData(customer)).ReturnsAsync(customer);
            //_addsvc.Verify(x => x.AddNewData(customer), Times.Once);
            Assert.Equal(customer, await _customerSvc.AddNewData(customer));
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

        [Fact(DisplayName = "CustomerSvc - Get list")]
        public async void GetCustomers()
        {
            //_readsvc.Setup(x => x.ReadDatas()).ReturnsAsync(customers);
            //_readsvc.Verify(x => x.ReadDatas(), Times.Once);
            Assert.NotNull(await _customerSvc.ReadDatas());
        }

        [Xunit.Theory(DisplayName = "CustomerSvc - Get list by email")]
        [InlineData("haotgps30117@fpt.edu.vn")]
        public async void GetCustomerByEmail(string email)
        {
            //_lookupsvc.Setup(x => x.GetDataByKey(email)).ReturnsAsync(customer);
            //_lookupsvc.Verify(x => x.GetDataByKey(email), Times.Once);
            var customer = await _customerSvc.GetDataByKey(email);
            Assert.Equal(email, customer.Email);
        }

        [Xunit.Theory(DisplayName = "CustomerSvc - Delete Sucessfully")]
        [InlineData("haotgps30117@fpt.edu.vn")]
        public async void DeleteCustomers(string email)
        {
            string ex = $"Xóa {email} thành công !";
            //_deletesvc.Setup(x => x.DeleteData(email)).ReturnsAsync(ex);
            //_deletesvc.Verify(x => x.DeleteData(email), Times.Once);
            Assert.Equal(ex, await _customerSvc.DeleteData(email));
        }
    }
}
