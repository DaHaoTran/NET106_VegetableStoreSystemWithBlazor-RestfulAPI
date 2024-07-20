using Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace DTO
{
    public class FastFoodDBContext : DbContext
    {
        public DbSet<Admin> admins { get; set; }
        public DbSet<Customer> customers { get; set; }
        public DbSet<CustomerInformation> customerInformations { get; set; }
        public DbSet<FoodCategory> foodCategories { get; set; }
        public DbSet<Food> foods { get; set; }
        public DbSet<Combo> combos { get; set; }
        public DbSet<ComboDetail> comboDetails { get; set; }
        public DbSet<Cart> carts { get; set; }
        public DbSet<CartItem> cartItems { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderItem> orderItems { get; set; }
        public DbSet<Guest> guests { get; set; }

        public FastFoodDBContext(DbContextOptions<FastFoodDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasKey(x => x.Email);
            modelBuilder.Entity<Food>().HasKey(x => x.FoodCode);
            modelBuilder.Entity<FoodCategory>().HasKey(x => x.FCategoryCode);
            modelBuilder.Entity<Combo>().HasKey(x => x.ComboCode);

            //Add Procedures
            modelBuilder
            .Entity<Food>()
            .ToTable(tb => tb.HasTrigger("AUTO_SET_SOLD"));
            modelBuilder
                .Entity<Cart>()
                .ToTable(tb => tb.HasTrigger("AUTO_CREATE_CART"));
            modelBuilder
              .Entity<Customer>()
              .ToTable(tb => tb.HasTrigger("AUTO_DELETE_RELATED"));
            modelBuilder
              .Entity<OrderItem>()
              .ToTable(tb => tb.HasTrigger("DECREASE_QUANTITY_FOOD"));
        }
    }
}
