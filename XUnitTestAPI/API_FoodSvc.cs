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
using NUnit.Framework;

namespace XUnitTestAPI
{
    public class API_FoodSvc
    {
        private Mock<IAddable<Customer>> _addsvc;
        private Mock<IEditable<Customer>> _editsvc;
        private Mock<IDeletable<string, Customer>> _deletesvc;
        private Mock<ILookupSvc<string, Customer>> _lookupsvc;
        private Mock<IReadable<Customer>> _readsvc;

        public API_FoodSvc()
        {
            _addsvc = new Mock<IAddable<Customer>>();
            _editsvc = new Mock<IEditable<Customer>>();
            _deletesvc = new Mock<IDeletable<string, Customer>>();
            _lookupsvc = new Mock<ILookupSvc<string, Customer>>();
            _readsvc = new Mock<IReadable<Customer>>();
        }

        [Fact(DisplayName = "CustomerSvc - Add")]
        public void AddCustomer()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps30117@fpt.edu.vn";
            customer.PassWord = "Giahao1";
            _addsvc.Setup(x => x.AddNewData(customer)).ReturnsAsync(customer);
            _addsvc.Verify(x => x.AddNewData(customer), Times.Once);
        }

        [Fact(DisplayName = "CustomerSvc - Edit")]
        public void EditCustomer()
        {
            Customer customer = new Customer();
            customer.Email = "haotgps30117@fpt.edu.vn";
            customer.PassWord = "Giahao200";
            _editsvc.Setup(x => x.EditData(customer)).ReturnsAsync(customer);
            _editsvc.Verify(x => x.EditData(customer), Times.Once);
        }

        [Fact(DisplayName = "CustomerSvc - GetList")]
        public void GetCustomers()
        {
            List<Customer> customers = new List<Customer>();
            _readsvc.Setup(x => x.ReadDatas()).ReturnsAsync(customers);
            _readsvc.Verify(x => x.ReadDatas(), Times.Once);
        }

        [Xunit.Theory(DisplayName = "CustomerSvc - GetList")]
        [InlineData("haotgps30117@fpt.edu.vn")]
        public void GetCustomerByEmail(string email)
        {
            Customer customer = new Customer();
            _lookupsvc.Setup(x => x.GetDataByKey(email)).ReturnsAsync(customer);
            _lookupsvc.Verify(x => x.GetDataByKey(email), Times.Once);
        }

        [Xunit.Theory(DisplayName = "CustomerSvc - GetList")]
        [InlineData("haotgps30117@fpt.edu.vn")]
        public void DeleteCustomers(string email)
        {
            string ex = $"Xóa {email} thành công !";
            _deletesvc.Setup(x => x.DeleteData(email)).ReturnsAsync(ex);
            _deletesvc.Verify(x => x.DeleteData(email), Times.Once);
        }
    }
}
