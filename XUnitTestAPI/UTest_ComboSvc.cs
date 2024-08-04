using API.Services.Implement;
using DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Models;
using NuGet.ContentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTestAPI
{
    public class UTest_ComboSvc
    {
        private ComboSvc _comboSvc;
        private DbContextOptions<FastFoodDBContext> _options;
        private FastFoodDBContext _dbContext;
        public UTest_ComboSvc()
        {
            _options = new DbContextOptionsBuilder<FastFoodDBContext>()
                .UseInMemoryDatabase(databaseName: "ASM_NET106_FastFood")
                .Options;
            _dbContext = new FastFoodDBContext(_options);
            if(_comboSvc == null ) _comboSvc = new ComboSvc(_dbContext);
        }

        //Add
        [Fact(DisplayName = "ComboSvc - Add combo sucessfully")]
        public async void AddComboSucessfully()
        {
            Combo combo = new Combo();
            combo.ComboName = "Best match combo";
            combo.CurrentPrice = 100;
            combo.Image = "03kjl2r203djd232.jpg";
            combo.ExpDate = DateTime.Parse("08/02/2024 12:00:00 AM");
            Assert.NotNull(await _comboSvc.AddNewData(combo));
        }

        [Theory(DisplayName = "ComboSvc - Add combo with exist name")]
        [InlineData("Mid-Autumn Festival combo")]
        public async void AddComboWithExistName(string name)
        {
            CreateExampleCombo();
            Combo combo = new Combo();
            combo.ComboName = name;
            combo.CurrentPrice = 30;
            combo.Image = "03kjl2r203djd232.jpg";
            combo.ExpDate = DateTime.Parse("08/02/2024 12:00:00 AM");
            Assert.Null(await _comboSvc.AddNewData(combo));
        }

        //GetList
        [Fact(DisplayName = "ComboSvc - Get combo list")]
        public async void GetCombos()
        {
            Assert.NotNull(await _comboSvc.ReadDatas());
        }

        //GetByCode
        [Theory(DisplayName = "ComboSvc - Get combo by code with not found code")]
        [InlineData("80a06707-f427-4947-af09-b60452f77467")]
        public async void GetComboByCodeWithNotFoundCode(Guid code)
        {
            Assert.Null(await _comboSvc.GetDataByKey(code));
        }

        [Fact(DisplayName = "ComboSvc - Get combo by code")]
        public async void GetComboByCode()
        {
            var code = await _dbContext.combos.Take(1).Select(x => x.ComboCode).FirstOrDefaultAsync();
            Assert.NotNull(await _comboSvc.GetDataByKey(code));
        }

        //GetListByName
        [Theory(DisplayName = "ComboSvc - Get combo list by name with not found name")]
        [InlineData("Tet festival combo")]
        public async void GetCombosByNameWithNotFoundName(string name)
        {
            Assert.Null(await _comboSvc.GetListByKey(name));
        }

        [Theory(DisplayName = "ComboSvc - Get combo list by name")]
        [InlineData("Mid-Autumn Festival combo")]
        public async void GetCombosByName(string name)
        {
            Assert.NotNull(await _comboSvc.GetListByKey(name));
        }

        //Edit
        [Theory(DisplayName = "ComboSvc - Edit combo with not found code")]
        [InlineData("80a06707-f427-4947-af09-b60452f77467")]
        public async void EditComboWithNotFoundCode(Guid code)
        {
            Combo combo = new Combo();
            combo.ComboCode = code;
            combo.ComboName = "School days combo";
            combo.CurrentPrice = 10;
            combo.Image = "dsalfkj32r32k2jl.jpg";
            combo.ExpDate = DateTime.Parse("08/04/2024 10:29:00 AM");
            Assert.Null(await _comboSvc.EditData(combo));
        }

        [Theory(DisplayName = "ComboSvc - Edit combo with exsit name other")]
        [InlineData("Best match combo")]
        public async void EditComboWithExistNameOther(string name)
        {
            Combo combo = new Combo();
            combo.ComboCode = await _dbContext.combos
                .Where(x => x.ComboName == name)
                .Select(x => x.ComboCode)
                .FirstOrDefaultAsync();
            combo.ComboName = "Mid-Autumn Festival combo";
            combo.CurrentPrice = 10;
            combo.Image = "dsalfkj32r32k2jl.jpg";
            combo.ExpDate = DateTime.Parse("08/04/2024 10:29:00 AM");
            Assert.Null(await _comboSvc.EditData(combo));
        }

        [Theory(DisplayName = "ComboSvc - Edit combo sucessfully")]
        [InlineData("Mid-Autumn Festival combo")]
        public async void EditComboSucessfully(string name)
        {
            Combo combo = new Combo();
            combo.ComboCode = await _dbContext.combos
                .Where(x => x.ComboName == name)
                .Select(x => x.ComboCode)
                .FirstOrDefaultAsync();
            combo.ComboName = "School days combo";
            combo.CurrentPrice = 10;
            combo.Image = "dsalfkj32r32k2jl.jpg";
            combo.ExpDate = DateTime.Parse("08/04/2024 10:29:00 AM");
            Assert.NotNull(await _comboSvc.EditData(combo));
        }

        //Delete
        [Theory(DisplayName = "ComboSvc - Delete combo with not found code")]
        [InlineData("80a06707-f427-4947-af09-b60452f77467")]
        public async void DeleteComboWithNotFoundCode(Guid code)
        {
            string ex = "Không tìm thấy";
            Assert.Equal(ex, await _comboSvc.DeleteData(code));
        }

        [Fact(DisplayName = "ComboSvc - Delete combo sucessfully")]
        public async void DeleteComboSucessfully()
        {
            var combo = await _dbContext.combos.Take(1).FirstAsync();
            string ex = $"Xóa {combo!.ComboCode} thành công !";
            Assert.Equal(ex, await _comboSvc.DeleteData(combo.ComboCode));
        }

        //Others
        private async void CreateExampleCombo()
        {
            Combo combo = new Combo();
            combo.ComboName = "Mid-Autumn Festival combo";
            combo.CurrentPrice = 30;
            combo.Image = "djlak32oj232j3i.jpg";
            combo.ExpDate = DateTime.Parse("08/02/2024 12:00:00 AM");
            await _comboSvc.AddNewData(combo);
        }
    }
}
