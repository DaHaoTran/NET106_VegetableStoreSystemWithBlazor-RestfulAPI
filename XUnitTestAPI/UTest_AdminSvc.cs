using API.Services.Implement;
using DTO;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Models;
using NuGet.Packaging.Licenses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace XUnitTestAPI
{
    public class UTest_AdminSvc
    {
        private AdminSvc _adminSvc;
        private DbContextOptions<FastFoodDBContext> _options;
        private FastFoodDBContext _dbContext;
        private AdminLoginSvc _loginSvc;
        public UTest_AdminSvc()
        {
            _options = new DbContextOptionsBuilder<FastFoodDBContext>()
                .UseInMemoryDatabase(databaseName: "NET106_ASM_FastFood")
                .Options;
            _dbContext = new FastFoodDBContext(_options);
            if(_adminSvc == null) _adminSvc = new AdminSvc(_dbContext);
            if(_loginSvc == null) _loginSvc = new AdminLoginSvc(_dbContext);
        }

        //Add
        [Fact(DisplayName = "AdminSvc - Add admin with null email", Skip = "No Catch Ex")]
        public void AddAdminWithNullEmail()
        {
            Admin admin = new Admin();
            admin.Password = "DaHao00";
            admin.Level = false;
            Action act = async () => await _adminSvc.AddNewData(admin);
            Assert.Throws<NoMatchFoundException>(act);
        }

        [Fact(DisplayName = "AdminSvc - Add admin with null password", Skip = "No Catch Ex")]
        public void AddAdminWithNullPassword()
        {
            Admin admin = new Admin();
            admin.Email = "haotgps30117@fpt.edu.vn";
            admin.Level = false;
            Action act = async () => await _adminSvc.AddNewData(admin);
            Assert.Throws<NoMatchFoundException>(act);
        }

        [Fact(DisplayName = "AdminSvc - Add admin with null level", Skip = "No Catch Ex")]
        public void AddAdminWithNullLevel()
        {
            Admin admin = new Admin();
            admin.Email = "haotgps30117@fpt.edu.vn";
            admin.Password = "DaHao00";
            Action act = async () => await _adminSvc.AddNewData(admin);
            Assert.Throws<NoMatchFoundException>(act);
        }

        [Fact(DisplayName = "AdminSvc - Add admin sucessfully with admin level")]
        public async void AddAdminWithAdminLevel()
        {
            Admin admin = new Admin();
            admin.Email = "haotgps30117@fpt.edu.vn";
            admin.Password = "DaHao00";
            admin.Level = true;
            Assert.Equal(admin, await _adminSvc.AddNewData(admin));
        }

        [Fact(DisplayName = "AdminSvc - Add admin sucessfully with staff level")]
        public async void AddAdminWithStaffLevel()
        {
            Admin admin = new Admin();
            admin.Email = "phongttps31457@fpt.edu.vn";
            admin.Password = "Thanhphong1";
            admin.Level = false;
            Assert.Equal(admin, await _adminSvc.AddNewData(admin));
        }

        [Fact(DisplayName = "AdminSvc - Add admin with exist email")]
        public async void AddAdminWithExistEmail()
        {
            Admin admin = new Admin();
            admin.Email = "phongttps31457@fpt.edu.vn";
            admin.Password = "Thanhphong1";
            admin.Level = false;
            Assert.Null(await _adminSvc.AddNewData(admin));
        }

        //GetList
        [Fact(DisplayName = "AdminSvc - Get admin list")]
        public async void GetAdmins()
        {
            Assert.NotNull(await _adminSvc.ReadDatas());
        }

        //GetByAdminCode
        [Theory(DisplayName = "AdminSvc - Get admin by code with null code", Skip = "No Catch Ex")]
        [InlineData(null)]
        public void GetAdminByCodeWithNullCode(string code)
        {
            Action action = async () => await _adminSvc.GetDataByKey(code);
            Assert.Throws<NotNullException>(action);
        }

        [Theory(DisplayName = "AdminSvc - Get admin by code with not found code")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public async void GetAdminByCodeWithNotFoundCode(Guid code)
        {
            Assert.Null(await _adminSvc.GetDataByKey(code));
        }

        [Fact(DisplayName = "AdminSvc - Get admin by code")]
        public async void GetAdminByCode()
        {
            var admin = await _dbContext.admins.Where(x => x.Email == "haotgps30117@fpt.edu.vn").FirstOrDefaultAsync();
            if(admin != default)
            {
                Assert.NotNull(await _adminSvc.GetDataByKey(admin.AdminCode));
            }
        }

        //GetByEmail
        [Theory(DisplayName = "AdminSvc - Get admin by email with null email", Skip = "No Catch Ex")]
        [InlineData(null)]
        public void GetAdminByEmailWithNullEmail(string email)
        {
            Action action = async () => await _adminSvc.GetDataByKey(email);
            Assert.Throws<NotNullException>(action);
        }

        [Theory(DisplayName = "AdminSvc - Get admin by email with not found email")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public async void GetAdminByEmailWithNotFoundEmail(string email)
        {
            Assert.Null(await _adminSvc.GetDataByKey(email));
        }

        [Theory(DisplayName = "AdminSvc - Get admin by email")]
        [InlineData("haotgps30117@fpt.edu.vn")]
        public async void GetAdminByEmail(string email)
        {
            Assert.NotNull(await _adminSvc.GetDataByKey(email));
        }

        //Edit
        [Theory(DisplayName = "AdminSvc - Edit admin with not found email")]
        [InlineData("kyhtps30117@fpt.edu.vn")]
        public async void EditAdminWithNotFoundEmail(string email)
        {
            Admin admin = new Admin();
            admin.Email = email;
            admin.Password = "Haotg23";
            admin.Level = true;
            Assert.Null(await _adminSvc.EditData(admin));
        }

        [Fact(DisplayName = "AdminSvc - Edit admin with offline state")]
        public async void EditAdminWithOfflineState()
        {
            await LogoutAdmin();
            Admin admin = new Admin();
            admin.Email = "haotgps30117@fpt.edu.vn";
            admin.Password = "Haotg23";
            admin.Level = true;
            Assert.NotNull(await _adminSvc.EditData(admin));
        }

        [Fact(DisplayName = "AdminSvc - Edit admin with null email", Skip = "No Catch Ex")]
        public void EditAdminWithNullEmail()
        {
            Admin admin = new Admin();
            admin.Email = string.Empty;
            admin.Password = "Haotg23";
            admin.Level = true;
            Action act = async () => await _adminSvc.EditData(admin);
            Assert.Throws<NoMatchFoundException>(act);
        }

        [Fact(DisplayName = "AdminSvc - Edit admin with null password", Skip = "No Catch Ex")]
        public void EditAdminWithNullPassword()
        {
            Admin admin = new Admin();
            admin.Email = "haotgps30117@fpt.edu.vn";
            admin.Password = string.Empty;
            admin.Level = true;
            Action act = async () => await _adminSvc.EditData(admin);
            Assert.Throws<NoMatchFoundException>(act);
        }

        [Fact(DisplayName = "AdminSvc - Edit admin with online state")]
        public async void EditAdminWithOnlineState()
        {
            Admin admin = await LoginAdmin();
            admin.Email = "haotgps30117@fpt.edu.vn";
            admin.Password = "Haotg23";
            admin.Level = true;
            Assert.Null(await _adminSvc.EditData(admin));
        }

        //Delete
        [Theory(DisplayName = "AdminSvc - Delete admin with null code", Skip = "No Catch Ex")]
        [InlineData(null)]
        public void DeleteAdminWithNullCode(Guid code)
        {
            Action act = async () => await _adminSvc.DeleteData(code);
            Assert.Throws<NoMatchFoundException>(act);
        }

        [Theory(DisplayName = "AdminSvc - Delete admin with not found code")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public async void DeleteAdminWithNotFoundCode(Guid code)
        {
            string ex = "Không tìm thấy";
            Assert.Equal(ex, await _adminSvc.DeleteData(code));
        }

        [Fact(DisplayName = "AdminSvc - Delete admin with online code")]
        public async void DeleteAdminWithIsOnlineCode()
        {
            Admin admin = await _dbContext.admins.Where(x => x.Email == "haotgps30117@fpt.edu.vn").FirstOrDefaultAsync();
            if(admin != default)
            {
                string ex = "Xóa thất bại, thuộc tính IsOnl đang là true !";
                Assert.Equal(ex, await _adminSvc.DeleteData(admin.AdminCode));
            }
        }

        [Fact(DisplayName = "AdminSvc - Delete admin with online state")]
        public async void DeleteAdminWithOnlineState()
        {
            var admin = await _dbContext.admins.Where(x => x.Email == "haotgps30117@fpt.edu.vn").FirstOrDefaultAsync();
            if (admin != default)
            {
                string ex = "Xóa thất bại, thuộc tính IsOnl đang là true !";
                Assert.Equal(ex, await _adminSvc.DeleteData(admin.AdminCode));
            }
        }

        [Fact(DisplayName = "AdminSvc - Delete admin sucessfully")]
        public async void DeleteAdminSucessfully()
        {
            await LogoutAdmin();
            var admin = await _dbContext.admins.Where(x => x.Email == "haotgps30117@fpt.edu.vn").FirstOrDefaultAsync();
            if (admin != default)
            {
                string ex = $"Xóa {admin.AdminCode} thành công !";
                Assert.Equal(ex, await _adminSvc.DeleteData(admin.AdminCode));
            }
        }

        //Others
        private async Task<Admin> LoginAdmin()
        {
            Admin admin = new Admin();
            admin.Email = "haotgps30117@fpt.edu.vn";
            admin.Password = "DaHao00";
            await _loginSvc.Login(admin);
            return admin;
        }

        private async Task<string> LogoutAdmin()
        {
            string email = "haotgps30117@fpt.edu.vn";
            await _loginSvc.Logout(email);
            return email;
        }
    }
}
