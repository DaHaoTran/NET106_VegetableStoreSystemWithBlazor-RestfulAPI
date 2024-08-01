using API.Services.Implement;
using API.Services.Interfaces;
using DTO;
using Microsoft.EntityFrameworkCore;
using Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "V1",
                    Title = "Tài liệu API dịch vụ kinh doanh thức ăn nhanh trên web",
                    Description = "localhost: https://localhost:7241/ \n" +
                    "Các thao tác sẵn có: \n" +
                    "- Tự động gán sold = 0 khi thêm mới thức ăn trong table foods database.\n" +
                    "- Tự động tạo id Cart khi thêm mới tài khoản Khách hàng trong table customers database. \n" +
                    "- Tự động xóa các thông tin liên quan của tài khoản khách hàng khi xóa khách hàng trong table customers database. \n" +
                    "- Tự động xóa các thông tin liên quan của tài khoản khách viếng thăm khi xóa khách viếng thăm trong table guests database. \n" +
                    "- Các mã sẽ được tự động tạo mỗi khi thêm mới. \n"
                });
                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            //AddDBContext
            builder.Services.AddDbContext<FastFoodDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("DTO"));
            });

            // Register Services
            // Admins 
            builder.Services.AddScoped<IAddable<Admin>, AdminSvc>();
            builder.Services.AddTransient<IReadable<Admin>, AdminSvc>();
            builder.Services.AddScoped<ILookupSvc<string, Admin>, AdminSvc>();
            builder.Services.AddScoped<ILookupSvc<Guid, Admin>, AdminSvc>();
            builder.Services.AddScoped<IEditable<Admin>, AdminSvc>();
            builder.Services.AddScoped<IDeletable<Guid, Admin>, AdminSvc>();
            //FoodCategory
            builder.Services.AddScoped<IAddable<FoodCategory>, FoodCategorySvc>();
            builder.Services.AddTransient<IReadable<FoodCategory>, FoodCategorySvc>();
            builder.Services.AddScoped<ILookupMoreSvc<string, FoodCategory>, FoodCategorySvc>();
            builder.Services.AddScoped<ILookupSvc<Guid, FoodCategory>, FoodCategorySvc>();
            builder.Services.AddScoped<IEditable<FoodCategory>, FoodCategorySvc>();
            builder.Services.AddScoped<IDeletable<Guid, FoodCategory>, FoodCategorySvc>();
            //Food
            builder.Services.AddScoped<IAddable<Food>, FoodSvc>();
            builder.Services.AddTransient<IReadable<Food>, FoodSvc>();
            builder.Services.AddScoped<ILookupMoreSvc<string, Food>, FoodSvc>();
            builder.Services.AddScoped<ILookupSvc<Guid, Food>, FoodSvc>();
            builder.Services.AddScoped<IEditable<Food>, FoodSvc>();
            builder.Services.AddScoped<IDeletable<Guid, Food>, FoodSvc>();
            //Combo
            builder.Services.AddTransient<IReadable<Combo>, ComboSvc>();
            builder.Services.AddScoped<IAddable<Combo>, ComboSvc>();
            builder.Services.AddScoped<IEditable<Combo>, ComboSvc>();
            builder.Services.AddScoped<IDeletable<Guid, Combo>, ComboSvc>();
            builder.Services.AddScoped<ILookupSvc<Guid, Combo>, ComboSvc>();
            builder.Services.AddScoped<ILookupMoreSvc<string, Combo>, ComboSvc>();
            //ComboDetail
            builder.Services.AddTransient<IReadable<ComboDetail>, ComboDetailSvc>();
            builder.Services.AddScoped<IAddable<ComboDetail>, ComboDetailSvc>();
            builder.Services.AddScoped<IEditable<ComboDetail>, ComboDetailSvc>();
            builder.Services.AddScoped<IDeletable<int, ComboDetail>, ComboDetailSvc>();
            builder.Services.AddScoped<ILookupMoreSvc<Guid, ComboDetail>, ComboDetailSvc>();
            builder.Services.AddScoped<ILookupSvc<int, ComboDetail>, ComboDetailSvc>();
            //Customer
            builder.Services.AddScoped<IAddable<Customer>, CustomerSvc>();
            builder.Services.AddTransient<IReadable<Customer>, CustomerSvc>();
            builder.Services.AddScoped<ILookupSvc<string, Customer>, CustomerSvc>();
            builder.Services.AddScoped<IEditable<Customer>, CustomerSvc>();
            builder.Services.AddScoped<IDeletable<string, Customer>, CustomerSvc>();
            //CustomerInformation
            builder.Services.AddScoped<ILookupSvc<int, CustomerInformation>, CustomerInformationSvc>();
            builder.Services.AddScoped<ILookupSvc<string, CustomerInformation>, CustomerInformationSvc>();
            builder.Services.AddScoped<ILookupMoreSvc<string, CustomerInformation>, CustomerInformationSvc>();
            builder.Services.AddScoped<IAddable<CustomerInformation>, CustomerInformationSvc>();
            builder.Services.AddTransient<IReadable<CustomerInformation>, CustomerInformationSvc>();
            builder.Services.AddScoped<IDeletable<int, CustomerInformation>, CustomerInformationSvc>();
            builder.Services.AddScoped<IEditable<CustomerInformation>, CustomerInformationSvc>();
            //Guest
            builder.Services.AddTransient<IReadable<Guest>, GuestSvc>();
            builder.Services.AddScoped<IAddable<Guest>, GuestSvc>();
            builder.Services.AddScoped<IDeletable<int, Guest>, GuestSvc>();
            builder.Services.AddScoped<ILookupSvc<int, Guest>, GuestSvc>();
            builder.Services.AddScoped<ILookupSvc<string, Guest>, GuestSvc>();
            builder.Services.AddScoped<ILookupMoreSvc<string, Guest>, GuestSvc>();
            //Order
            builder.Services.AddScoped<IDeletable<Guid, Order>, OrderSvc>();
            builder.Services.AddScoped<ILookupMoreSvc<string, Order>, OrderSvc>();
            builder.Services.AddTransient<IReadable<Order>, OrderSvc>();
            builder.Services.AddScoped<IEditable<Order> , OrderSvc>();
            builder.Services.AddScoped<IAddable<Order>, OrderSvc>();
            //OrderItem
            builder.Services.AddScoped<IAddable<List<OrderItem>>, OrderItemSvc>();
            builder.Services.AddScoped<ILookupMoreSvc<Guid, OrderItem> , OrderItemSvc>();
            //Login
            builder.Services.AddScoped<ILoginSvc<Admin>, AdminLoginSvc>();
            builder.Services.AddScoped<ILoginSvc<Customer>, CustomerLoginSvc>();
            //Cart
            builder.Services.AddScoped<ILookupSvc<string, Cart>, CartSvc>();
            //Cart Item
            builder.Services.AddScoped<IAddable<CartItem>, CartItemSvc>();
            builder.Services.AddScoped<IDeletable<int, CartItem>, CartItemSvc>();
            builder.Services.AddScoped<IEditable<CartItem>, CartItemSvc>();
            builder.Services.AddScoped<ILookupMoreSvc<int, CartItem>, CartItemSvc>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors("CorsPolicy");

            app.MapControllers();

            app.Run();
        }
    }
}
