using API.Services.Implement;
using API.Services.Interfaces;
using DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.InMemory;
using Models;

namespace XUnitTestAPI
{
    public class UTest_LoginAdminSvc
    {
        private AdminLoginSvc _loginsvc;
        private AdminSvc _adminSvc;
        private FastFoodDBContext _dbContext;
        private DbContextOptions<FastFoodDBContext> _options;

        public UTest_LoginAdminSvc()
        {
            _options = new DbContextOptionsBuilder<FastFoodDBContext>()
            .UseInMemoryDatabase(databaseName: "NET106_ASM_FastFood")
            .Options;

            _dbContext = new FastFoodDBContext(_options);
            if(_loginsvc == null) _loginsvc = new AdminLoginSvc(_dbContext);
            if(_adminSvc == null) _adminSvc = new AdminSvc(_dbContext);
            CreateNewAdmin();
        }

        //Login
        [Fact(DisplayName = "LoginAdminSvc - Login with null email")]
        public async void LoginAdminWithNullEmail()
        {
            Admin admin = new Admin();
            admin.Email = "haotgps30117@fpt.edu.vn";
            admin.Password = string.Empty;
            Assert.False(await _loginsvc.Login(admin));
        }

        [Fact(DisplayName = "LoginAdminSvc - Login with null password")]
        public async void LoginAdminWithNullPassword()
        {
            Admin admin = new Admin();
            admin.Email = string.Empty;
            admin.Password = "Giahao1";
            Assert.False(await _loginsvc.Login(admin));
        }

        [Fact(DisplayName = "LoginAdminSvc - Login with wrong email")]
        public async void LoginAdminWithWrongEmail()
        {
            Admin admin = new Admin();
            admin.Email = "haotgps30117@gmail.com";
            admin.Password = "Giahao1";
            Assert.False(await _loginsvc.Login(admin));
        }

        [Fact(DisplayName = "LoginAdminSvc - Login with wrong password")]
        public async void LoginAdminWithWrongPassword()
        {
            Admin admin = new Admin();
            admin.Email = "haotgps30117@fpt.edu.vn";
            admin.Password = "Giahao2";
            Assert.False(await _loginsvc.Login(admin));
        }

        [Fact(DisplayName = "LoginAdminSvc - Login Sucessfully")]
        public async void LoginAdminSucessfully()
        {
            Admin admin = new Admin();
            admin.Email = "haotgps30117@fpt.edu.vn";
            admin.Password = "Giahao1";
            Assert.True(await _loginsvc.Login(admin));
        }

        //Logout
        [Theory(DisplayName = "LoginAdminSvc - Logout with null email")]
        [InlineData(null)]
        public async void LogoutAdminWithNullEmail(string email)
        {
            Assert.False(await _loginsvc.Logout(email));
        }

        [Theory(DisplayName = "LoginAdminSvc - Logout with not found email")]
        [InlineData("haotgps30118@fpt.edu.vn")]
        public async void LogoutAdminWithNotFoundEmail(string email)
        {
            Assert.False(await _loginsvc.Logout(email));
        }

        [Theory(DisplayName = "LoginAdminSvc - Logout sucessfully")]
        [InlineData("haotgps30117@fpt.edu.vn")]
        public async void LogoutAdminSucessfully(string email)
        {
            Assert.True(await _loginsvc.Logout(email));
        }

        //Others
        [Fact(Skip = "Function")]
        private async void CreateNewAdmin()
        {
            Admin admin = new Admin();
            admin.Email = "haotgps30117@fpt.edu.vn";
            admin.Password = "Giahao1";
            admin.Level = true;
            await _adminSvc.AddNewData(admin);
        }
    }
}
