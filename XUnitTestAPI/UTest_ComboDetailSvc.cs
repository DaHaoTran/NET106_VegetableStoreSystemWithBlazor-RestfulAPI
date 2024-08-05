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
    public class UTest_ComboDetailSvc
    {
        private ComboDetailSvc _detailSvc;
        private FoodSvc _foodSvc;
        private ComboSvc _comboSvc;
        private DbContextOptions<FastFoodDBContext> _options;
        private FastFoodDBContext _dbContext;
        private static List<Guid> _foodCodeList = new List<Guid>();
        private static Guid _comboCode;
        public UTest_ComboDetailSvc()
        {
            _options = new DbContextOptionsBuilder<FastFoodDBContext>()
                .UseInMemoryDatabase(databaseName: "NET106_ASM_FastFood")
                .Options;
            _dbContext = new FastFoodDBContext(_options);
            if(_detailSvc == null) _detailSvc = new ComboDetailSvc(_dbContext);
            if(_foodSvc == null) _foodSvc = new FoodSvc(_dbContext);
            if(_comboSvc == null) _comboSvc = new ComboSvc(_dbContext);
            CreateNewListFood();
        }

        [Fact(DisplayName = "ComboDetailSvc - Add combo detail sucessfully")]
        public async void CreateComboDetailSucessfully()
        {
            ComboDetail detail = new ComboDetail();
            detail.FoodCode = _foodCodeList[0];
            detail.ComboCode = _comboCode;
            Assert.NotNull(await _detailSvc.AddNewData(detail));
        }

        [Fact(DisplayName = "ComboDetailSvc - Add combo detail with exist combo detail")]
        public async void CreateComboDetailWithExistComboDetail()
        {
            CreateExampleComboDetail();
            ComboDetail detail = new ComboDetail();
            detail.FoodCode = _foodCodeList[1];
            detail.ComboCode = _comboCode;
            Assert.Null(await _detailSvc.AddNewData(detail));
        }

        //GetList
        [Fact(DisplayName = "ComboDetailSvc - Get combo detail list")]
        public async void GetComboDetails()
        {
            Assert.NotNull(await _detailSvc.ReadDatas());
        }

        //GetListByComboCode
        [Theory(DisplayName = "ComboDetailSvc - Get combo detail list by combo code with not found combo code")]
        [InlineData("6d6ec53e-f88d-4723-8eb1-34bd724c265b")]
        public async void GetComboDetailsByComboCodeWithNotFoundComboCode(Guid foodCode)
        {
            Assert.Null(await _detailSvc.GetListByKey(foodCode));
        }

        [Fact(DisplayName = "ComboDetailSvc - Get combo detail list by combo code")]
        public async void GetComboDetailsByComboCode()
        {
            Assert.NotNull(await _detailSvc.GetListByKey(_comboCode));
        }

        //GetById
        [Theory(DisplayName = "ComboDetailSvc - Get combo detail by id with not found id")]
        [InlineData(4)]
        public async void GetComboDetailByIdWithNotFoundId(int id)
        {
            Assert.Null(await _detailSvc.GetDataByKey(id));
        }

        [Fact(DisplayName = "ComboDetailSvc - Get combo detail by id")]
        public async void GetComboDetailById()
        {
            var id = await _dbContext.comboDetails.Select(x => x.Id).FirstOrDefaultAsync();
            Assert.NotNull(await _detailSvc.GetDataByKey(id));
        }

        //Edit
        [Theory(DisplayName = "ComboDetailSvc - Edit combo detail by id with not found id")]
        [InlineData(4)]
        public async void EditComboDetailWithNotFoundId(int id)
        {
            ComboDetail detail = new ComboDetail();
            detail.Id = id;
            detail.FoodCode = _foodCodeList[7];
            detail.ComboCode = _comboCode;
            Assert.Null(await _detailSvc.EditData(detail));
        }

        [Theory(DisplayName = "ComboDetailSvc - Edit combo detail sucessfully")]
        [InlineData(1)]
        public async void EditComboDetailSucessfully(int id)
        {
            ComboDetail detail = new ComboDetail();
            detail.Id = id;
            detail.FoodCode = _foodCodeList[7];
            detail.ComboCode = _comboCode;
            Assert.NotNull(await _detailSvc.EditData(detail));
        }

        //Delete
        [Theory(DisplayName = "ComboDetailSvc - Delete combo detail by id with not found id")]
        [InlineData(3)]
        public async void DeleteComboDetailWithNotFoundId(int id)
        {
            string ex = "Không tìm thấy";
            Assert.Equal(ex, await _detailSvc.DeleteData(id));
        }

        [Fact(DisplayName = "ComboDetailSvc - Delete combo detail sucessfully")]
        public async void DeleteComboDetailSucessfully()
        {
            var id = await _dbContext.comboDetails.Select(x => x.Id).FirstOrDefaultAsync();
            string ex = $"Xóa {id} thành công !";
            Assert.Equal(ex, await _detailSvc.DeleteData(id));
        }

        //Others
        private async void CreateNewCombo()
        {
            Combo combo = new Combo();
            combo.ComboName = "Chirtmas combo";
            combo.CurrentPrice = 100;
            combo.Image = "03kjl2r203djd232.jpg";
            combo.ExpDate = DateTime.Parse("08/02/2024 12:00:00 AM");
            await _comboSvc.AddNewData(combo);
            _comboCode = await _dbContext.combos.Select(x => x.ComboCode).FirstOrDefaultAsync();
        }
        private async void CreateNewListFood()
        {
            var count = 0;
            for(int i = 1; i < 10; i++)
            {
                count+=i;
                Food food = new Food();
                food.FoodName = "Food " + count;
                food.CurrentPrice = 100;
                food.Left = 10;
                food.Image = "0dfajslkfd3jl223.jpg";
                food.FCategoryCode = Guid.Parse("39abb280-1850-4750-9660-a7e81d9af83e");
                food.AdminCode = Guid.Parse("0049408c-2c42-4401-b7b6-3dcd16e71ad3");
                await _foodSvc.AddNewData(food);
            }
            _foodCodeList = await _dbContext.foods.Select(x => x.FoodCode).ToListAsync();
            CreateNewCombo();
        }
        private async void CreateExampleComboDetail()
        {
            ComboDetail detail = new ComboDetail();
            detail.FoodCode = _foodCodeList[1];
            detail.ComboCode = _comboCode;
            await _detailSvc.AddNewData(detail);
        }
    }
}
