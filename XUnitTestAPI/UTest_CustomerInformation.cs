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
        }

        //Add
        [Theory(DisplayName = "CustomerInformationSvc - Add failed with not found email")]
        [InlineData("haotgps20204@fpt.edu.vn")]
        public async void AddCustomerInformationWithNotFoundEmail(string email)
        {
            CreateNewCustomer();
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
            CreateNewCustomer();
            CustomerInformation information = new CustomerInformation();
            information.CustomerName = "Gia Hào";
            information.Address = "Công viên phần mềm Quang Trung";
            information.PhoneNumber = "0392837461";
            information.CustomerEmail = "haotgps30117@fpt.edu.vn";
            Assert.Equal(information, await _informationSvc.AddNewData(information));
        }

        //Edit
        [Fact(DisplayName = "CustomerInformationSvc - Edit failed with not found id")]
        public async void EditCustomerInformationWithNotFoundId()
        {
            CreateNewCustomer();
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
            CreateNewCustomer();
            CustomerInformation information = new CustomerInformation();
            var cus = await _dbContext.customerInformations
                .Where(x => x.CustomerEmail == "haotgps30117@fpt.edu.vn").FirstOrDefaultAsync();
            int id = cus!.CInforId;
            information.CInforId = id;
            information.CustomerName = "Trần Gia Hào";
            information.Address = "Công viên phần mềm Quang Trung, Quận 12, TP.HCM";
            information.PhoneNumber = "0392837461";
            information.CustomerEmail = "haotgps30117@fpt.edu.vn";
            Assert.Equal(information, await _informationSvc.EditData(information));
        }

        //Delete
        [Fact(DisplayName = "CustomerInformationSvc - Delete failed with not found id")]
        public async void DeleteCustomerInformationWithNotFoundId()
        {
            CreateNewCustomer();
            CustomerInformation information = new CustomerInformation();
            var cus = await _dbContext.customerInformations
                .Where(x => x.CustomerEmail == "haotgps30117@fpt.edu.vn").FirstOrDefaultAsync();
            int id = cus!.CInforId + 1;
            string ex = "Không tìm thấy";
            Assert.Equal(ex, await _informationSvc.DeleteData(id));
        }

        [Fact(DisplayName = "CustomerInformationSvc - Delete sucessfully")]
        public async void DeleteCustomerInformationSucessfully()
        {
            CreateNewCustomer();
            CustomerInformation information = new CustomerInformation();
            var cus = await _dbContext.customerInformations
                .Where(x => x.CustomerEmail == "haotgps30117@fpt.edu.vn").FirstOrDefaultAsync();
            int id = cus!.CInforId;
            string ex = $"Xóa {id} thành công !";
            Assert.Equal(ex, await _informationSvc.DeleteData(id));
        }

        //Others
        private async void CreateNewCustomer()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps30117@fpt.edu.vn";
            customer.PassWord = "Giahao1";
            await _customerSvc.AddNewData(customer);
        }
    }
}
