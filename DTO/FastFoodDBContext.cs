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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if(!optionsBuilder.IsConfigured)
            //{
            //    optionsBuilder.UseSqlServer("Data Source=HAOTRAN;Initial Catalog=NET106_ASM_FastFood;Integrated Security=True;Trust Server Certificate=True", b => b.MigrationsAssembly("DTO"));
            //}
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<CustomerInformation>()
            //    .HasOne<Customer>(x => x.Customer)
            //    .WithMany(x => x.CustomerInformations)
            //    .HasForeignKey(x => x.CustomerEmail);

            //modelBuilder.Entity<Customer>()
            //    .HasOne<Admin>(x => x.Admins)
            //    .WithMany(x => x.customers)
            //    .HasForeignKey(x => x.AdminCode);

            //modelBuilder.Entity<Customer>()
            //    .HasOne<Cart>(x => x.Cart)
            //    .WithOne(x => x.Customer);
            modelBuilder.Entity<Customer>().HasKey(x => x.Email);
            modelBuilder.Entity<Food>().HasKey(x => x.FoodCode);
            modelBuilder.Entity<FoodCategory>().HasKey(x => x.FCategoryCode);
            modelBuilder.Entity<Combo>().HasKey(x => x.ComboCode);
        }
    }
}
