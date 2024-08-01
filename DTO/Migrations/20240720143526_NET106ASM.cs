using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTO.Migrations
{
    /// <inheritdoc />
    public partial class NET106ASM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admins",
                columns: table => new
                {
                    AdminCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "Varchar(200)", nullable: false),
                    Password = table.Column<string>(type: "Varchar(100)", nullable: false),
                    IsOnl = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Level = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admins", x => x.AdminCode);
                });

            migrationBuilder.CreateTable(
                name: "combos",
                columns: table => new
                {
                    ComboCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComboName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CurrentPrice = table.Column<int>(type: "int", nullable: false),
                    PreviousPrice = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<string>(type: "Varchar(Max)", nullable: false),
                    ApplyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_combos", x => x.ComboCode);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    Email = table.Column<string>(type: "Varchar(200)", nullable: false),
                    PassWord = table.Column<string>(type: "Varchar(100)", nullable: false),
                    UserName = table.Column<string>(type: "Varchar(300)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "foodCategories",
                columns: table => new
                {
                    FCategoryCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_foodCategories", x => x.FCategoryCode);
                });

            migrationBuilder.CreateTable(
                name: "carts",
                columns: table => new
                {
                    CartId = table.Column<int>(type: "int", nullable: false),
                    CustomerEmail = table.Column<string>(type: "Varchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carts", x => x.CartId);
                    table.ForeignKey(
                        name: "FK_carts_customers_CustomerEmail",
                        column: x => x.CustomerEmail,
                        principalTable: "customers",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "customerInformations",
                columns: table => new
                {
                    CInforId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PhoneNumber = table.Column<string>(type: "Char(10)", maxLength: 10, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerEmail = table.Column<string>(type: "Varchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customerInformations", x => x.CInforId);
                    table.ForeignKey(
                        name: "FK_customerInformations_customers_CustomerEmail",
                        column: x => x.CustomerEmail,
                        principalTable: "customers",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    OrderCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comment = table.Column<string>(type: "Nvarchar(Max)", nullable: true),
                    PaymentMethod = table.Column<string>(type: "Nvarchar(100)", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false),
                    CInforId = table.Column<int>(type: "int", nullable: false),
                    CustomerEmail = table.Column<string>(type: "Varchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.OrderCode);
                    table.ForeignKey(
                        name: "FK_orders_customers_CustomerEmail",
                        column: x => x.CustomerEmail,
                        principalTable: "customers",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "foods",
                columns: table => new
                {
                    FoodCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FoodName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CurrentPrice = table.Column<int>(type: "int", nullable: false),
                    PreviousPrice = table.Column<int>(type: "int", nullable: false),
                    Left = table.Column<int>(type: "int", nullable: false),
                    Sold = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<string>(type: "Varchar(Max)", nullable: false),
                    FCategoryCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_foods", x => x.FoodCode);
                    table.ForeignKey(
                        name: "FK_foods_admins_AdminCode",
                        column: x => x.AdminCode,
                        principalTable: "admins",
                        principalColumn: "AdminCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_foods_foodCategories_FCategoryCode",
                        column: x => x.FCategoryCode,
                        principalTable: "foodCategories",
                        principalColumn: "FCategoryCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "guests",
                columns: table => new
                {
                    GuesId = table.Column<int>(type: "int", nullable: false),
                        //.Annotation("SqlServer:Identity", "1, 1"),
                    GuestName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PhoneNumber = table.Column<string>(type: "Char(10)", maxLength: 10, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guests", x => x.GuesId);
                    table.ForeignKey(
                        name: "FK_guests_orders_OrderCode",
                        column: x => x.OrderCode,
                        principalTable: "orders",
                        principalColumn: "OrderCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cartItems",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                        //.Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CartId = table.Column<int>(type: "int", nullable: false),
                    FoodCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cartItems", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_cartItems_carts_CartId",
                        column: x => x.CartId,
                        principalTable: "carts",
                        principalColumn: "CartId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cartItems_foods_FoodCode",
                        column: x => x.FoodCode,
                        principalTable: "foods",
                        principalColumn: "FoodCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comboDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    FoodCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComboCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comboDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_comboDetails_combos_ComboCode",
                        column: x => x.ComboCode,
                        principalTable: "combos",
                        principalColumn: "ComboCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comboDetails_foods_FoodCode",
                        column: x => x.FoodCode,
                        principalTable: "foods",
                        principalColumn: "FoodCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orderItems",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    OrderCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FoodCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderItems", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_orderItems_foods_FoodCode",
                        column: x => x.FoodCode,
                        principalTable: "foods",
                        principalColumn: "FoodCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orderItems_orders_OrderCode",
                        column: x => x.OrderCode,
                        principalTable: "orders",
                        principalColumn: "OrderCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cartItems_CartId",
                table: "cartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_cartItems_FoodCode",
                table: "cartItems",
                column: "FoodCode");

            migrationBuilder.CreateIndex(
                name: "IX_carts_CustomerEmail",
                table: "carts",
                column: "CustomerEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_comboDetails_ComboCode",
                table: "comboDetails",
                column: "ComboCode");

            migrationBuilder.CreateIndex(
                name: "IX_comboDetails_FoodCode",
                table: "comboDetails",
                column: "FoodCode");

            migrationBuilder.CreateIndex(
                name: "IX_customerInformations_CustomerEmail",
                table: "customerInformations",
                column: "CustomerEmail");

            migrationBuilder.CreateIndex(
                name: "IX_foods_AdminCode",
                table: "foods",
                column: "AdminCode");

            migrationBuilder.CreateIndex(
                name: "IX_foods_FCategoryCode",
                table: "foods",
                column: "FCategoryCode");

            migrationBuilder.CreateIndex(
                name: "IX_guests_OrderCode",
                table: "guests",
                column: "OrderCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orderItems_FoodCode",
                table: "orderItems",
                column: "FoodCode");

            migrationBuilder.CreateIndex(
                name: "IX_orderItems_OrderCode",
                table: "orderItems",
                column: "OrderCode");

            migrationBuilder.CreateIndex(
                name: "IX_orders_CustomerEmail",
                table: "orders",
                column: "CustomerEmail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cartItems");

            migrationBuilder.DropTable(
                name: "comboDetails");

            migrationBuilder.DropTable(
                name: "customerInformations");

            migrationBuilder.DropTable(
                name: "guests");

            migrationBuilder.DropTable(
                name: "orderItems");

            migrationBuilder.DropTable(
                name: "carts");

            migrationBuilder.DropTable(
                name: "combos");

            migrationBuilder.DropTable(
                name: "foods");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "admins");

            migrationBuilder.DropTable(
                name: "foodCategories");

            migrationBuilder.DropTable(
                name: "customers");
        }
    }
}
