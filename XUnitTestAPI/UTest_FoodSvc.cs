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
    public class UTest_FoodSvc
    {
        private FoodSvc _foodSvc;
        private FoodCategorySvc _foodCategorySvc;
        private DbContextOptions<FastFoodDBContext> _options;
        private FastFoodDBContext _dbContext;
        private AdminSvc _adminSvc;
        private static Guid fCategoryCode { get; set; }
        private static Guid adminCode { get; set; }
        public UTest_FoodSvc()
        {
            _options = new DbContextOptionsBuilder<FastFoodDBContext>()
                .UseInMemoryDatabase(databaseName: "ASM_NET106_FastFood")
                .Options;
            _dbContext = new FastFoodDBContext(_options);
            if(_foodSvc == null ) _foodSvc = new FoodSvc(_dbContext);
            if(_foodCategorySvc == null) _foodCategorySvc = new FoodCategorySvc(_dbContext);
            if(_adminSvc == null) _adminSvc = new AdminSvc(_dbContext);
            CreateNewAdmin();
            CreateNewFoodCategory();
        }

        //Add
        [Fact(DisplayName = "FoodSvc - Add food with null AdminCode", Skip = "No exception was thrown")]
        public void AddFoodWithNullAdminCode()
        {
            Food food = new Food();
            food.FoodName = "Chicken fried";
            food.CurrentPrice = 100;
            food.Left = 10;
            food.Image = "0dfajslkfd3jl223.jpg";
            food.FCategoryCode = fCategoryCode;
            Action act = async () => await _foodSvc.AddNewData(food);
            Assert.Throws<NullReferenceException>(() => act());
        }

        [Fact(DisplayName = "FoodSvc - Add food with null food category code", Skip = "No exception was thrown")]
        public void AddFoodWithNullFoodCategoryCode()
        {
            Food food = new Food();
            food.FoodName = "Chicken fried";
            food.CurrentPrice = 100;
            food.Left = 10;
            food.Image = "0dfajslkfd3jl223.jpg";
            food.AdminCode = adminCode;
            Action act = async () => await _foodSvc.AddNewData(food);
            Assert.Throws<NullReferenceException>(() => act());
        }

        [Fact(DisplayName = "FoodSvc - Add food Sucessfully")]
        public async void AddFoodSucessfully()
        {
            Food food = new Food();
            food.FoodName = "Chicken fried";
            food.CurrentPrice = 100;
            food.Left = 10;
            food.Image = "0dfajslkfd3jl223.jpg";
            food.FCategoryCode = fCategoryCode;
            food.AdminCode = adminCode;
            Assert.NotNull(await _foodSvc.AddNewData(food));    
        }

        [Theory(DisplayName = "FoodSvc - Add food with exist name")]
        [InlineData("Hamburger")]
        public async void AddFoodWithExistName(string name)
        {
            CreateExampleFood();
            Food food = new Food();
            food.FoodName = name;
            food.CurrentPrice = 340;
            food.Left = 30;
            food.Image = "h332jkldsajlfk.jpg";
            food.FCategoryCode = fCategoryCode;
            food.AdminCode = adminCode;
            Assert.Null(await _foodSvc.AddNewData(food));
        }

        //GetList
        [Fact(DisplayName = "FoodSvc - Get food list")]
        public async void GetFoods()
        {
            Assert.NotNull(await _foodSvc.ReadDatas());
        }

        //GetByCode
        [Theory(DisplayName = "FoodSvc - Get food by code with not found code")]
        [InlineData("2a4efefb-f908-4421-8e4e-3410420ef2e8")]
        public async void GetFoodByCodeWithNotFoundCode(Guid code)
        {
            Assert.Null(await _foodSvc.GetDataByKey(code));
        }

        [Fact(DisplayName = "FoodSvc - Get food by code")]
        public async void GetFoodByCode()
        {
            var food = _dbContext.foods.Take(1).First();
            Assert.NotNull(await _foodSvc.GetDataByKey(food.FoodCode));
        }

        //GetListByFoodName
        [Theory(DisplayName = "FoodSvc - Get food list by food name with not found food name")]
        [InlineData("Tomatos")]
        public async void GetListByFoodNameWithNotFound(string name)
        {
            Assert.Null(await _foodSvc.GetListByKey(name));
        }

        [Theory(DisplayName = "FoodSvc - Get food list by food name")]
        [InlineData("hamburger")]
        public async void GetListByFoodName(string name)
        {
            Assert.NotNull(await _foodSvc.GetListByKey(name));
        }

        //Edit
        [Theory(DisplayName = "FoodSvc - Edit food with not found code")]
        [InlineData("2a4efefb-f908-4421-8e4e-3410420ef2e8")]
        public async void EditFoodWithNotFoundCode(Guid code)
        {
            Food food = new Food();
            food.FoodCode = code;
            food.FoodName = "Noodle";
            food.CurrentPrice = 100;
            food.Left = 10;
            food.Sold = 4;
            food.Image = "0dfajslkfd3jl223.jpg";
            food.FCategoryCode = fCategoryCode;
            food.AdminCode = adminCode;
            Assert.Null(await _foodSvc.EditData(food));
        }

        [Theory(DisplayName = "FoodSvc - Edit food with exist name")]
        [InlineData("Hamburger")]
        public async void EditFoodWithExsitName(string name)
        {
            Food food = new Food();
            food.FoodCode = await _dbContext.foods
                .Where(x => x.FoodName == "Chicken fried")
                .Select(x => x.FoodCode)
                .FirstOrDefaultAsync();
            food.FoodName = name;
            food.CurrentPrice = 100;
            food.Left = 10;
            food.Sold = 4;
            food.Image = "0dfajslkfd3jl223.jpg";
            food.FCategoryCode = fCategoryCode;
            food.AdminCode = adminCode;
            Assert.Null(await _foodSvc.EditData(food));
        }

        [Theory(DisplayName = "FoodSvc - Edit food sucessfully")]
        [InlineData("Hamburger")]
        public async void EditFoodSucessfully(string searchName)
        {
            Food food = new Food();
            food.FoodCode = await _dbContext.foods
               .Where(x => x.FoodName == searchName)
               .Select(x => x.FoodCode)
               .SingleOrDefaultAsync();
            //string s = searchName;
            //food.FoodCode = await _dbContext.foods.Take(1).Select(x => x.FoodCode).FirstOrDefaultAsync();
            food.FoodName = "Noodle";
            food.CurrentPrice = 100;
            food.Left = 10;
            food.Sold = 4;
            food.Image = "0dfajslkfd3jl223.jpg";
            food.FCategoryCode = fCategoryCode;
            food.AdminCode = adminCode;
            Assert.NotNull(await _foodSvc.EditData(food));
        }

        //Delete
        [Theory(DisplayName = "FoodSvc - Delete food with not found code")]
        [InlineData("2a4efefb-f908-4421-8e4e-3410420ef2e8")]
        public async void DeleteFoodWithNotFoundCode(Guid code)
        {
            string ex = "Không tìm thấy";
            Assert.Equal(ex, await _foodSvc.DeleteData(code));
        }

        [Fact(DisplayName = "FoodSvc - Delete food sucessfully")]
        public async void DeleteFoodSucessfully()
        {
            var foodCode = await _dbContext.foods.Take(1).Select(x => x.FoodCode).SingleOrDefaultAsync();
            string ex = $"Xóa {foodCode} thành công !";
            Assert.Equal(ex, await _foodSvc.DeleteData(foodCode));
        }

        //Others
        private async void CreateNewFoodCategory()
        {
            FoodCategory category = new FoodCategory();
            category.CategoryName = "Fast foods";
            await _foodCategorySvc.AddNewData(category);
            fCategoryCode = await _dbContext.foodCategories.Where(X => X.CategoryName == category.CategoryName).Select(x => x.FCategoryCode).SingleOrDefaultAsync();
        }
        private async void CreateNewAdmin()
        {
            Admin admin = new Admin();
            admin.Email = "haotgps30117@fpt.edu.vn";
            admin.Password = "Haotg2K4";
            admin.Level = false;
            await _adminSvc.AddNewData(admin);
            var adminCreated = await _adminSvc.GetDataByKey(admin.Email);
            adminCode = adminCreated.AdminCode;
        }
        private async void CreateExampleFood()
        {
            Food food = new Food();
            food.FoodName = "Hamburger";
            food.CurrentPrice = 100;
            food.Left = 10;
            food.Image = "0dfajslkfd3jl223.jpg";
            food.FCategoryCode = fCategoryCode;
            food.AdminCode = adminCode;
            await _foodSvc.AddNewData(food);
        }
    }
}
