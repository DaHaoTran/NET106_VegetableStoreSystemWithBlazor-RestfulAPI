using API.Services.Implement;
using DTO;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTestAPI
{
    public class UTest_LoginCustomerSvc
    {
        private CustomerLoginSvc _loginsvc;
        private CustomerSvc _customerSvc;
        private FastFoodDBContext _dbContext;
        private DbContextOptions<FastFoodDBContext> _options;

        public UTest_LoginCustomerSvc()
        {
            _options = new DbContextOptionsBuilder<FastFoodDBContext>()
            .UseInMemoryDatabase(databaseName: "NET106_ASM_FastFood")
            .Options;

            _dbContext = new FastFoodDBContext(_options);
            if (_loginsvc == null) _loginsvc = new CustomerLoginSvc(_dbContext);
            if (_customerSvc == null) _customerSvc = new CustomerSvc(_dbContext);
            CreateNewCustomer();
        }

        //Login
        [Fact(DisplayName = "LoginCustomerSvc - Login with null email")]
        public async void LogincustomerWithNullEmail()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps30117@fpt.edu.vn";
            customer.PassWord = string.Empty;
            Assert.False(await _loginsvc.Login(customer));
        }

        [Fact(DisplayName = "LogincustomerSvc - Login with null PassWord")]
        public async void LogincustomerWithNullPassWord()
        {
            Customer customer = new Customer();
            customer.Email = string.Empty;
            customer.PassWord = "Giahao1";
            Assert.False(await _loginsvc.Login(customer));
        }

        [Fact(DisplayName = "LogincustomerSvc - Login with wrong email")]
        public async void LogincustomerWithWrongEmail()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps30117@gmail.com";
            customer.PassWord = "Giahao1";
            Assert.False(await _loginsvc.Login(customer));
        }

        [Fact(DisplayName = "LogincustomerSvc - Login with wrong PassWord")]
        public async void LogincustomerWithWrongPassWord()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps30117@fpt.edu.vn";
            customer.PassWord = "Giahao2";
            Assert.False(await _loginsvc.Login(customer));
        }

        [Fact(DisplayName = "LogincustomerSvc - Login Sucessfully")]
        public async void LogincustomerSucessfully()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps30117@fpt.edu.vn";
            customer.PassWord = "Giahao1";
            Assert.True(await _loginsvc.Login(customer));
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
