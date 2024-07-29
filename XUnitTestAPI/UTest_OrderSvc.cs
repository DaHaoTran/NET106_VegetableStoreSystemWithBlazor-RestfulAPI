using API.Services.Implement;
using DTO;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTestAPI
{
    public class UTest_OrderSvc
    {
        private OrderSvc _orderSvc;
        private CustomerSvc _customerSvc;
        private CustomerInformationSvc _informationSvc;
        private DbContextOptions<FastFoodDBContext> _options;
        private FastFoodDBContext _dbContext;
        private int inforId {  get; set; }
        private static Guid orderCode { get; set;  }
        public UTest_OrderSvc()
        {
            _options = new DbContextOptionsBuilder<FastFoodDBContext>()
                .UseInMemoryDatabase(databaseName: "NET106_ASM_FastFood")
                .Options;
            _dbContext = new FastFoodDBContext(_options);
            if(_orderSvc == null) _orderSvc = new OrderSvc(_dbContext);
            if(_customerSvc == null) _customerSvc = new CustomerSvc(_dbContext);
            if(_informationSvc == null) _informationSvc = new CustomerInformationSvc(_dbContext);
            CreateNewCustomer();
        }

        //Add
        [Fact(DisplayName = "OrderSvc - Add order with null payment method", Skip = "No catch ex")]
        public void AddOrderWithNullPaymentMethod()
        {
            Order order = new Order();
            order.Total = 1000000;
            order.Comment = "Test";
            order.CInforId = inforId;
            order.CustomerEmail = "bachdb30284@fpt.edu.vn";
            Action act = async () => await _orderSvc.AddNewData(order);
            Assert.Throws<ValidationException>(act);
        }

        [Fact(DisplayName = "OrderSvc - Add order with null information id", Skip = "No catch ex")]
        public void AddOrderWithNullInforId()
        {
            Order order = new Order();
            order.PaymentMethod = "Thanh toán khi nhận hàng";
            order.Total = 1000000;
            order.Comment = "Test";
            order.CustomerEmail = "bachdb30284@fpt.edu.vn";
            Action act = async () => await _orderSvc.AddNewData(order);
            Assert.Throws<ValidationException>(act);
        }

        [Fact(DisplayName = "OrderSvc - Add order with null total", Skip = "No catch ex")]
        public void AddOrderWithNullTotal()
        {
            Order order = new Order();
            order.PaymentMethod = "Thanh toán khi nhận hàng";
            order.Comment = "Test";
            order.CInforId = inforId;
            order.CustomerEmail = "bachdb30284@fpt.edu.vn";
            Action act = async () => await _orderSvc.AddNewData(order);
            Assert.Throws<ValidationException>(act);
        }

        [Fact(DisplayName = "OrderSvc - Add order with null customer email", Skip = "No catch ex")]
        public void AddOrderWithNullCustomerEmail()
        {
            Order order = new Order();
            order.PaymentMethod = "Thanh toán khi nhận hàng";
            order.Total = 1000000;
            order.Comment = "Test";
            order.CInforId = inforId;
            Action act = async () => await _orderSvc.AddNewData(order);
            Assert.Throws<ValidationException>(act);
        }

        [Fact(DisplayName = "OrderSvc - Add order sucessfully")]
        public async void AddOrderSucessfully()
        {
            Order order = new Order();
            order.PaymentMethod = "Thanh toán khi nhận hàng";
            order.Total = 1000000;
            order.Comment = "Test";
            order.CInforId = inforId;
            order.CustomerEmail = "bachdb30284@fpt.edu.vn";
            Assert.NotNull(await _orderSvc.AddNewData(order));
            var ordernew = await _dbContext.orders.Where(x => x.CustomerEmail == order.CustomerEmail).FirstOrDefaultAsync();
            orderCode = ordernew!.OrderCode;
        }

        //GetList
        [Fact(DisplayName = "OrderSvc - Get order list")]
        public async void GetOrders()
        {
            Assert.NotNull(await _orderSvc.ReadDatas());
        }

        //GetListByEmail
        [Theory(DisplayName = "OrderSvc - Get order list by email")]
        [InlineData("haotgps30117@fpt.edu.vn")]
        public async void GetOrdersByEmailWithNotFoundEmail(string email)
        {
            Assert.Null(await _orderSvc.GetListByKey(email));
        }

        [Theory(DisplayName = "OrderSvc - Get order list by email")]
        [InlineData("bachdb30284@fpt.edu.vn")]
        public async void GetOrdersByEmail(string email)
        {
            Assert.Null(await _orderSvc.GetListByKey(email));
        }

        //Edit
        [Theory(DisplayName = "OrderSvc - Edit order with not found order code")]
        [InlineData("1dbd031f-90b3-407f-b2a0-ac934b1d4b0e")]
        public async void EditOrderWithNotFoundOrderCode(Guid code)
        {
            Order order = new Order();
            order.State = "Ongoing delivered";
            order.OrderCode = code;
            Assert.Null(await _orderSvc.EditData(order));
        }

        [Fact(DisplayName = "OrderSvc - Edit order with null state", Skip = "No catch ex")]
        public async void EditOrderWithNullState()
        {
            Order order = new Order();
            order.OrderCode = orderCode;
            Action act = async () => await _orderSvc.EditData(order);
            Assert.Throws<NullReferenceException>(act);
        }

        [Fact(DisplayName = "OrderSvc - Edit order with ongoing delivered state")]
        public async void EditOrderWithOngoingDeliveredState()
        {
            Order order = new Order();
            order.OrderCode = orderCode;
            order.State = "Ongoing delivered";
            Assert.NotNull(await _orderSvc.EditData(order));
        }

        [Fact(DisplayName = "OrderSvc - Edit order with delivered state")]
        public async void EditOrderWithDeliveredState()
        {
            Order order = new Order();
            order.OrderCode = orderCode;
            order.State = "Delivered";
            await _orderSvc.EditData(order);
            var orderEdited = await _dbContext.orders.Where(x => x.OrderCode == orderCode).FirstOrDefaultAsync();           
            Assert.NotNull(orderEdited!.DeliveryDate);
        }

        //Delete
        [Theory(DisplayName = "OrderSvc - Delete order with not found order code")]
        [InlineData("1dbd031f-90b3-407f-b2a0-ac934b1d4b0e")]
        public async void DeleteOrderWithNotFoundOrderCode(Guid code)
        {
            var ex = "Không tìm thấy";
            Assert.Equal(ex, await _orderSvc.DeleteData(code));
        }

        [Fact(DisplayName = "OrderSvc - Delete order sucessfully")]
        public async void DeleteOrderSucessfully()
        {
            var ex = $"Xóa {orderCode} thành công !";
            Assert.Equal(ex, await _orderSvc.DeleteData(orderCode));
        }

        //Others
        private async void CreateNewCustomer()
        {
            Customer customer = new Customer();
            customer.Email = "bachdb30284@fpt.edu.vn";
            customer.PassWord = "doanbb2K4";
            await _customerSvc.AddNewData(customer);
            CreateNewCustomerInformation();
        }
        private async void CreateNewCustomerInformation()
        {
            CustomerInformation information = new CustomerInformation();
            information.CustomerName = "Đoàn Bá Bách";
            information.Address = "Quận Gò Vấp";
            information.PhoneNumber = "0837461234";
            information.CustomerEmail = "bachdb30284@fpt.edu.vn";
            await _informationSvc.AddNewData(information);
            var infor = await _dbContext.customerInformations.Where(x => x.CustomerEmail == "bachdb30284@fpt.edu.vn").FirstOrDefaultAsync();
            inforId = infor!.CInforId;
        }
    }
}
