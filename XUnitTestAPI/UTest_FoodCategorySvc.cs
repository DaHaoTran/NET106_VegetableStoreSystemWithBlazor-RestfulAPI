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
    public class UTest_FoodCategorySvc
    {
        private FoodCategorySvc _categorySvc;
        private DbContextOptions<FastFoodDBContext> _options;
        private FastFoodDBContext _dbContext;
        private static Guid codeCreated {  get; set; }

        public UTest_FoodCategorySvc()
        {
            _options = new DbContextOptionsBuilder<FastFoodDBContext>()
                .UseInMemoryDatabase(databaseName: "ASM_NET106_FastFood").
                Options;
            _dbContext = new FastFoodDBContext(_options);
            if(_categorySvc == null) _categorySvc = new FoodCategorySvc(_dbContext);
        }

        //Add
        [Theory(DisplayName = "FoodCategorySvc - Add food category with exsit category name")]
        [InlineData("fast foods")]
        public async void AddFoodCategoryWithExistCategoryName(string name)
        {
            CreateNewFoodCategory();
            FoodCategory foodCategory = new FoodCategory();
            foodCategory.CategoryName = name;
            Assert.Null(await _categorySvc.AddNewData(foodCategory));
            codeCreated = await _dbContext.foodCategories.Where(x => x.CategoryName == foodCategory.CategoryName).Select(x => x.FCategoryCode).SingleOrDefaultAsync();
        }

        [Theory(DisplayName = "FoodCategorySvc - Add food category sucessully")]
        [InlineData("Drinks")]
        public async void AddFoodCategorySucessfully(string name)
        {
            FoodCategory foodCategory = new FoodCategory();
            foodCategory.CategoryName = name;
            Assert.NotNull(await _categorySvc.AddNewData(foodCategory));
        }

        //GetList
        [Fact(DisplayName = "FoodCategorySvc - Get food category list")]
        public async void GetFoodCategories()
        {
            Assert.NotNull(await _categorySvc.ReadDatas());
        }

        //GetByCategoryCode
        [Theory(DisplayName = "FoodCategorySvc - Get food category by code with not found code")]
        [InlineData("1068a2be-50d6-4eb5-89b0-53cb42bb74a3")]
        public async void GetFoodCategoryByCodeWithNotFoundCode(Guid code)
        {
            Assert.Null(await _categorySvc.GetDataByKey(code));
        }

        [Fact(DisplayName = "FoodCategorySvc - Get food category by code")]
        public async void GetFoodCategoryByCode()
        {
            Assert.NotNull(await _categorySvc.GetDataByKey(codeCreated));
        }

        //GetListByName
        [Theory(DisplayName = "FoodCategorySvc - Get food category list by name with not found name")]
        [InlineData("Vegetables")]
        public async void GetFoodCategoriesByNameWithNotFoundName(string name)
        {
            Assert.Null(await _categorySvc.GetListByKey(name));
        }

        [Theory(DisplayName = "FoodCategorySvc - Get food category list by name")]
        [InlineData("Drinks")]
        public async void GetFoodCategoriesByName(string name)
        {
            Assert.NotNull(await _categorySvc.GetListByKey(name));
        }

        //Edit
        [Fact(DisplayName = "FoodCategorySvc - Edit food category with not found code")]
        public async void EditFoodCategoryWithNotFoundCode()
        {
            FoodCategory category = new FoodCategory();
            category.FCategoryCode = Guid.Parse("1068a2be-50d6-4eb5-89b0-53cb42bb74a3");
            category.CategoryName = "Fast foods";
            Assert.Null(await _categorySvc.EditData(category));
        }

        [Theory(DisplayName = "FoodCategorySvc - Edit food category with exist name")]
        [InlineData("Fast foods")]
        public async void EditFoodCategoryWithExistName(string name)
        {
            CreateNewFoodCategory();
            FoodCategory category = new FoodCategory();
            category.FCategoryCode = codeCreated;
            category.CategoryName = name;
            Assert.Null(await _categorySvc.EditData(category));
        }

        [Theory(DisplayName = "FoodCategorySvc - Edit food category sucessfully")]
        [InlineData("Vegetables")]
        public async void EditFoodCategorySucessfully(string name)
        {
            CreateNewFoodCategory();
            FoodCategory category = new FoodCategory();
            category.FCategoryCode = codeCreated;
            category.CategoryName = name;
            Assert.NotNull(await _categorySvc.EditData(category));
        }

        //Delete
        [Theory(DisplayName = "FoodCategorySvc - Delete food category with not found code")]
        [InlineData("1068a2be-50d6-4eb5-89b0-53cb42bb74a3")]
        public async void DeleteFoodCategoryWithNotFoundCode(Guid code)
        {
            string ex = "Không tìm thấy";
            Assert.Equal(ex, await _categorySvc.DeleteData(code));
        }

        [Fact(DisplayName = "FoodCategorySvc - Delete food category sucessfully")]
        public async void DeleteFoodCategorySucessfully()
        {
            string ex = $"Xóa {codeCreated} thành công !";
            Assert.Equal(ex, await _categorySvc.DeleteData(codeCreated));
        }

        //Others
        private async void CreateNewFoodCategory()
        {
            FoodCategory newCategory = new FoodCategory();
            newCategory.CategoryName = "fast foods";
            await _categorySvc.AddNewData(newCategory);
        }
    }
}
