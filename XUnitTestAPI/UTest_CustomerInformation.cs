using API.Services.Implement;
using DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.InMemory;
using Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace XUnitTestAPI
{
    public class UTest_CustomerInformation
    {
        private CustomerInformationSvc _informationSvc;
        private CustomerSvc _customerSvc;
        private FastFoodDBContext _dbContext;
        private DbContextOptions<FastFoodDBContext> _options;
        public UTest_CustomerInformation() 
        {
            _options = new DbContextOptionsBuilder<FastFoodDBContext>()
                .UseInMemoryDatabase(databaseName: "NET106_ASM_FastFood")
                .Options;
            _dbContext = new FastFoodDBContext(_options);
            if(_informationSvc == null) _informationSvc = new CustomerInformationSvc(_dbContext);
            if(_customerSvc == null) _customerSvc = new CustomerSvc(_dbContext);
            CreateNewCustomer();
            CreateNewInformation();
        }

        //Add
        [Theory(DisplayName = "CustomerInformationSvc - Add failed with not found email")]
        [InlineData("haotgps20204@fpt.edu.vn")]
        public async void AddCustomerInformationWithNotFoundEmail(string email)
        {
            CustomerInformation information = new CustomerInformation();
            information.CustomerName = "Gia Hào";
            information.Address = "Công viên phần mềm Quang Trung";
            information.PhoneNumber = "0392837461";
            information.CustomerEmail = email;
            Assert.Null(await _informationSvc.AddNewData(information));
        }

        [Fact(DisplayName = "CustomerInformationSvc - Add sucessfully")]
        public async void AddCustomerInformationSucessfully()
        {
            CustomerInformation information = new CustomerInformation();
            information.CustomerName = "Trung Kỳ";
            information.Address = "Quận Gò Vấp";
            information.PhoneNumber = "0837461234";
            information.CustomerEmail = "haotgps30117@fpt.edu.vn";
            Assert.NotNull(await _informationSvc.AddNewData(information));
        }

        //Edit
        [Fact(DisplayName = "CustomerInformationSvc - Edit failed with not found id")]
        public async void EditCustomerInformationWithNotFoundId()
        {
            CustomerInformation information = new CustomerInformation();
            var cus = await _dbContext.customerInformations
                .Where(x => x.CustomerEmail == "haotgps30117@fpt.edu.vn").FirstOrDefaultAsync();
            int id = cus!.CInforId + 1;
            information.CInforId = id;
            information.CustomerName = "Gia Hào";
            information.Address = "Công viên phần mềm Quang Trung";
            information.PhoneNumber = "0392837461";
            information.CustomerEmail = "haotgps30117@fpt.edu.vn";
            Assert.Null(await _informationSvc.EditData(information));
        }

        [Fact(DisplayName = "CustomerInformationSvc - Edit sucessfully")]
        public async void EditCustomerInformationSucessfully()
        {
            CustomerInformation information = new CustomerInformation();
            var cus = await _dbContext.customerInformations
                .Where(x => x.CustomerEmail == "haotgps30117@fpt.edu.vn").FirstOrDefaultAsync();
            int id = cus!.CInforId;
            information.CInforId = id;
            information.CustomerName = "Trần Gia Hào";
            information.Address = "Công viên phần mềm Quang Trung, Quận 12, TP.HCM";
            information.PhoneNumber = "0392837461";
            information.CustomerEmail = "haotgps30117@fpt.edu.vn";
            Assert.NotNull(await _informationSvc.EditData(information));
        }

        //Delete
        [Fact(DisplayName = "CustomerInformationSvc - Delete failed with not found id")]
        public async void DeleteCustomerInformationWithNotFoundId()
        {
            var cus = await _dbContext.customerInformations
                .Where(x => x.CustomerEmail == "haotgps30117@fpt.edu.vn").FirstOrDefaultAsync();
            int id = cus!.CInforId + 1;
            string ex = "Không tìm thấy";
            Assert.Equal(ex, await _informationSvc.DeleteData(id));
        }

        [Fact(DisplayName = "CustomerInformationSvc - Delete sucessfully")]
        public async void DeleteCustomerInformationSucessfully()
        {
            var cus = await _dbContext.customerInformations
                .Where(x => x.CustomerEmail == "haotgps30117@fpt.edu.vn").FirstOrDefaultAsync();
            int id = cus!.CInforId;
            string ex = $"Xóa {id} thành công !";
            Assert.Equal(ex, await _informationSvc.DeleteData(id));
        }

        //GetList
        [Fact(DisplayName = "CustomerInformationSvc - Get list")]
        public async void GetCustomerInfomations()
        {
            Assert.NotNull(await _informationSvc.ReadDatas());
        }

        //GetById
        [Fact(DisplayName = "CustomerInformationSvc - Get by id with null")]
        public async void GetCustomerInformationByIdWithNull()
        {
            Assert.Null(await _informationSvc.GetDataByKey(null!));
        }

        [Fact(DisplayName = "CustomerInformationSvc - Get by id with not found id")]
        public async void GetCustomerInfomationByIdWithNotFound()
        {
            int id = 10;
            Assert.Null(await _informationSvc.GetDataByKey(id));
        }

        [Fact(DisplayName = "CustomerInfomationSvc - Get by id")]
        public async void GetCustomerInformationById()
        {
            var cus = await _dbContext.customerInformations
               .Where(x => x.CustomerEmail == "haotgps30117@fpt.edu.vn").FirstOrDefaultAsync();
            int id = cus!.CInforId;
            Assert.NotNull(await _informationSvc.GetDataByKey(id)); 
        }

        //GetListByRelated
        [Fact(DisplayName = "CustomerInformationSvc - Get list by related information with null", Skip = "No catch ex")]
        public void GetCustomerInformationsWithNull()
        {
            Action act = async () => await _informationSvc.GetListByKey(null!);
            Assert.Throws<ArgumentNullException>(() => act());  
        }

        [Theory(DisplayName = "CustomerInformationSvc - Get list by related information with not found")]
        [InlineData("haotgps30118@fpt.edu.vn")]
        public async void GetCustomerInformationsWithNotFound(string email)
        {
            Assert.Null(await _informationSvc.GetListByKey(email));
        }

        [Theory(DisplayName = "CustomerInformationSvc - Get list by related information with email")]
        [InlineData("haotgps30117@fpt.edu.vn")]
        public async void GetCustomerInfomationsByEmail(string email)
        {
            Assert.NotNull(await _informationSvc.GetListByKey(email));
        }

        [Theory(DisplayName = "CustomerInformationSvc - Get list by related information with name")]
        [InlineData("Hào")]
        public async void GetCustomerInfomationsByName(string name)
        {
            Assert.NotNull(await _informationSvc.GetListByKey(name));
        }

        [Theory(DisplayName = "CustomerInformationSvc - Get list by related information with address")]
        [InlineData("Quang Trung")]
        public async void GetCustomerInfomationsByAddress(string address)
        {
            Assert.NotNull(await _informationSvc.GetListByKey(address));
        }

        //GetByPhoneNumber
        [Theory(DisplayName = "CustomerInformationSvc - Get by phone number with null")]
        [InlineData(null)]
        public async void GetCustomerInformationByPhoneNumberWithNull(string phoneNumber)
        {
            Assert.Null(await _informationSvc.GetDataByKey(phoneNumber));
        }

        [Theory(DisplayName = "CustomerInformationSvc - Get by phone number with not found")]
        [InlineData("0394734331")]
        public async void GetCustomerInformationByPhoneNumberWithNotFound(string phoneNumber)
        {
            Assert.Null(await _informationSvc.GetDataByKey(phoneNumber));
        }

        [Theory(DisplayName = "CustomerInformationSvc - Get by phone number")]
        [InlineData("0392837461")]
        public async void GetCustomerInformationByPhoneNumber(string phoneNumber)
        {
            Assert.NotNull(await _informationSvc.GetDataByKey(phoneNumber));
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

        [Fact(Skip = "Function")]
        private async void CreateNewInformation()
        {
            CustomerInformation information = new CustomerInformation();
            information.CustomerName = "Gia Hào";
            information.Address = "Công viên phần mềm Quang Trung";
            information.PhoneNumber = "0392837461";
            information.CustomerEmail = "haotgps30117@fpt.edu.vn";
            await _informationSvc.AddNewData(information);
        }
    }
}
