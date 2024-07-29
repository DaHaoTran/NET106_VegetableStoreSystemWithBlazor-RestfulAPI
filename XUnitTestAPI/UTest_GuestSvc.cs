using API.Services.Implement;
using DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace XUnitTestAPI
{
    public class UTest_GuestSvc
    {
        private GuestSvc _guestSvc;
        private OrderSvc _orderSvc;
        private DbContextOptions<FastFoodDBContext> _options;
        private FastFoodDBContext _dBContext;
        private static Guid orderCode { get; set; }   
        public UTest_GuestSvc()
        {
            _options = new DbContextOptionsBuilder<FastFoodDBContext>()
                .UseInMemoryDatabase(databaseName: "NET106_ASM_FastFood")
                .Options;
            _dBContext = new FastFoodDBContext(_options);
            if(_guestSvc == null ) _guestSvc = new GuestSvc(_dBContext);
            if(_orderSvc == null) _orderSvc = new OrderSvc(_dBContext);
            CreateNewOrder();
        }

        //Add
        [Fact(DisplayName = "GuestSvc - Add guest sucessfully")]
        public async void AddGuestSucessfully()
        {
            Guest guest = new Guest();
            guest.GuestName = "Huỳnh Trung Kỳ";
            guest.OrderCode = orderCode;
            guest.PhoneNumber = "0948372612";
            guest.Address = "Phường Đông Hưng Thuận, Quận 12, TP.HCM";
            Assert.NotNull(await _guestSvc.AddNewData(guest));
        }

        //GetList
        [Fact(DisplayName = "GuestSvc - Get guest list")]
        public async void GetGuests()
        {
            Assert.NotNull(await _guestSvc.ReadDatas());
        }

        //GetById
        [Theory(DisplayName = "GuestSvc - Get guest by guest id with not found id")]
        [InlineData(2)]
        public async void GetGuestByGuestIdWithNotFoundId(int id)
        {
            Assert.Null(await _guestSvc.GetDataByKey(id));
        }

        [Theory(DisplayName = "GuestSvc - Get guest by guest id")]
        [InlineData(1)]
        public async void GetGuestByGuestId(int id)
        {
            Assert.NotNull(await _guestSvc.GetDataByKey(id));
        }

        //GetByPhoneNumber
        [Theory(DisplayName = "GuestSvc - Get guest by phone number with not found phone number")]
        [InlineData("0837463212")]
        public async void GetGuestByPhoneNumberWithNotFoundPhone(string phoneNumber)
        {
            Assert.Null(await _guestSvc.GetDataByKey(phoneNumber));
        }

        [Theory(DisplayName = "GuestSvc - Get guest by phone number with not found phone number")]
        [InlineData("0948372612")]
        public async void GetGuestByPhoneNumber(string phoneNumber)
        {
            Assert.NotNull(await _guestSvc.GetDataByKey(phoneNumber));
        }

        //GetListByRelatedInformation
        [Theory(DisplayName = "GuestSvc - Get guest list by related information with not found")]
        [InlineData("Đường Phạm Văn Đồng, quận 12, TP.HCM")]
        public async void GetGuestsByRelatedInformationWithNotFoundInformation(string related)
        {
            Assert.Null(await _guestSvc.GetListByKey(related));
        }

        [Theory(DisplayName = "GuestSvc - Get guest list by related information with guest name")]
        [InlineData("Trung Kỳ")]
        public async void GetGuestsByRelatedInformationWithGuestName(string related)
        {
            Assert.NotNull(await _guestSvc.GetListByKey(related));
        }

        [Theory(DisplayName = "GuestSvc - Get guest list by related information with address")]
        [InlineData("quận 12")]
        public async void GetGuestsByRelatedInformationWithAddress(string related)
        {
            Assert.NotNull(await _guestSvc.GetListByKey(related));
        }

        //Delete
        [Theory(DisplayName = "GuestSvc - Delete guest with not found id")]
        [InlineData(2)]
        public async void DeleteGuestWithNotFoundGuestId(int guestId)
        {
            string ex = "Không tìm thấy";
            Assert.Equal(ex, await _guestSvc.DeleteData(guestId));
        }

        [Theory(DisplayName = "GuestSvc - Delete guest with not found id")]
        [InlineData(1)]
        public async void DeleteGuestSucessfully(int guestId)
        {
            string ex = $"Xóa {guestId} thành công !";
            Assert.Equal(ex, await _guestSvc.DeleteData(guestId));
        }

        //Others
        private async void CreateNewOrder()
        {
            Order order = new Order();
            order.PaymentMethod = "Thanh toán khi nhận hàng";
            order.Total = 1000000;
            order.Comment = "Test";
            order.CInforId = 1;
            order.CustomerEmail = "bachdb30284@fpt.edu.vn";
            await _orderSvc.AddNewData(order);
            orderCode = await _dBContext.orders.Where(x => x.CustomerEmail == order.CustomerEmail).Select(x => x.OrderCode).FirstOrDefaultAsync();
        }
    }
}
