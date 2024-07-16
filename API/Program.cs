using API.Context;
using API.Services.Implement;
using API.Services.Interfaces;
using DTO;
using Microsoft.EntityFrameworkCore;
using Models;
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
            builder.Services.AddSwaggerGen();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "V1",
                    Title = "Tài liệu API dịch vụ kinh doanh thức ăn nhanh trên web",
                    Description = "localhost: https://localhost:7241/"
                });
                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            //AddDBContext
            builder.Services.AddDbContext<FastFoodDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Register Services
            // Admins 
            builder.Services.AddSingleton<IAddable<Admin>, AdminSvc>();
            builder.Services.AddTransient<IReadable<Admin>, AdminSvc>();
            builder.Services.AddSingleton<ILookupSvc<string, Admin>, AdminSvc>();
            builder.Services.AddSingleton<ILookupSvc<Guid, Admin>, AdminSvc>();
            builder.Services.AddSingleton<IEditable<Admin>, AdminSvc>();
            builder.Services.AddSingleton<IDeletable<Guid, Admin>, AdminSvc>();
            //FoodCategory
            builder.Services.AddSingleton<IAddable<FoodCategory>, FoodCategorySvc>();
            builder.Services.AddTransient<IReadable<FoodCategory>, FoodCategorySvc>();
            builder.Services.AddSingleton<ILookupSvc<string, FoodCategory>, FoodCategorySvc>();
            builder.Services.AddSingleton<ILookupSvc<Guid, FoodCategory>, FoodCategorySvc>();
            builder.Services.AddSingleton<IEditable<FoodCategory>, FoodCategorySvc>();
            builder.Services.AddSingleton<IDeletable<Guid, FoodCategory>, FoodCategorySvc>();
            //Food
            builder.Services.AddTransient<IAddable<Food>, FoodSvc>();
            builder.Services.AddScoped<IReadable<Food>, FoodSvc>();
            builder.Services.AddTransient<ILookupSvc<string, Food>, FoodSvc>();
            builder.Services.AddTransient<IEditable<Food>, FoodSvc>();
            builder.Services.AddTransient<IDeletable<string, Food>, FoodSvc>();
            //Customer
            builder.Services.AddSingleton<IAddable<Customer>, CustomerSvc>();
            builder.Services.AddTransient<IReadable<Customer>, CustomerSvc>();
            builder.Services.AddSingleton<ILookupSvc<string, Customer>, CustomerSvc>();
            builder.Services.AddSingleton<IEditable<Customer>, CustomerSvc>();
            builder.Services.AddSingleton<IDeletable<string, Customer>, CustomerSvc>();
            //CustomerInformation
            builder.Services.AddSingleton<ILookupSvc<int, CustomerInformation>, CustomerInformationSvc>();
            builder.Services.AddSingleton<ILookupMoreSvc<string, CustomerInformation>, CustomerInformationSvc>();
            builder.Services.AddSingleton<IAddable<CustomerInformation>, CustomerInformationSvc>();
            builder.Services.AddTransient<IReadable<CustomerInformation>, CustomerInformationSvc>();
            builder.Services.AddSingleton<IDeletable<int, CustomerInformation>, CustomerInformationSvc>();
            builder.Services.AddSingleton<IEditable<CustomerInformation>, CustomerInformationSvc>();
            //Guest
            builder.Services.AddTransient<IReadable<Guest>, GuestSvc>();
            builder.Services.AddSingleton<IAddable<Guest>, GuestSvc>();
            builder.Services.AddSingleton<IDeletable<int, Guest>, GuestSvc>();
            builder.Services.AddSingleton<ILookupSvc<int, Guest>, GuestSvc>();
            //Order
            builder.Services.AddScoped<IReadableHasWhere<string, Order>, OrderSvc>();
            builder.Services.AddTransient<ILookupSvc<int, Order>, OrderSvc>();
            builder.Services.AddTransient<IEditable<Order> , OrderSvc>();
            builder.Services.AddTransient<IAddable<Order>, OrderSvc>();
            //OrderItem
            builder.Services.AddScoped<IReadableHasWhere<int, OrderItem>, OrderItemSvc>();
            builder.Services.AddTransient<IAddable<OrderItem>, OrderItemSvc>();
            //Login
            builder.Services.AddTransient<ILoginSvc<Admin>, AdminLoginSvc>();
            builder.Services.AddTransient<ILoginSvc<Customer>, CustomerLoginSvc>();
            //Cart
            builder.Services.AddTransient<ILookupSvc<string, Cart>, CartSvc>();
            //Cart Item
            builder.Services.AddTransient<IAddable<CartItem>, CartItemSvc>();
            builder.Services.AddTransient<IReadableHasWhere<int, CartItem>, CartItemSvc>();
            builder.Services.AddTransient<IDeletable<List<CartItem>, CartItem>, CartItemSvc>();
            builder.Services.AddTransient<IEditable<CartItem>, CartItemSvc>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
