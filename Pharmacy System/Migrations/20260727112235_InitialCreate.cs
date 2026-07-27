using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmacy_System.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    CustomerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DOB = table.Column<DateOnly>(type: "date", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.CustomerID);
                });

            migrationBuilder.CreateTable(
                name: "pharmacies",
                columns: table => new
                {
                    PharmacyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PharmacyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    StorageCapacity = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pharmacies", x => x.PharmacyID);
                });

            migrationBuilder.CreateTable(
                name: "suppliers",
                columns: table => new
                {
                    SupplierID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suppliers", x => x.SupplierID);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "warehouses",
                columns: table => new
                {
                    WarehouseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouses", x => x.WarehouseID);
                });

            migrationBuilder.CreateTable(
                name: "medicines",
                columns: table => new
                {
                    MedicineID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medicines", x => x.MedicineID);
                    table.ForeignKey(
                        name: "FK_medicines_categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "categories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "customerOrders",
                columns: table => new
                {
                    CustomerOrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    PharmacyId = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customerOrders", x => x.CustomerOrderId);
                    table.ForeignKey(
                        name: "FK_customerOrders_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customerOrders_pharmacies_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "pharmacies",
                        principalColumn: "PharmacyID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pharmacists",
                columns: table => new
                {
                    PharmacistID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    PharmacyID = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pharmacists", x => x.PharmacistID);
                    table.ForeignKey(
                        name: "FK_pharmacists_pharmacies_PharmacyID",
                        column: x => x.PharmacyID,
                        principalTable: "pharmacies",
                        principalColumn: "PharmacyID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pharmacists_users_UserID",
                        column: x => x.UserID,
                        principalTable: "users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pharmacyStocks",
                columns: table => new
                {
                    PharmacyStockID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PharmacyID = table.Column<int>(type: "int", nullable: false),
                    MedicineID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pharmacyStocks", x => x.PharmacyStockID);
                    table.ForeignKey(
                        name: "FK_pharmacyStocks_medicines_MedicineID",
                        column: x => x.MedicineID,
                        principalTable: "medicines",
                        principalColumn: "MedicineID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pharmacyStocks_pharmacies_PharmacyID",
                        column: x => x.PharmacyID,
                        principalTable: "pharmacies",
                        principalColumn: "PharmacyID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Supplies",
                columns: table => new
                {
                    SupplyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SupplyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "date", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SupplierID = table.Column<int>(type: "int", nullable: false),
                    MedicineID = table.Column<int>(type: "int", nullable: false),
                    WarehouseID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplies", x => x.SupplyId);
                    table.ForeignKey(
                        name: "FK_Supplies_medicines_MedicineID",
                        column: x => x.MedicineID,
                        principalTable: "medicines",
                        principalColumn: "MedicineID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Supplies_suppliers_SupplierID",
                        column: x => x.SupplierID,
                        principalTable: "suppliers",
                        principalColumn: "SupplierID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Supplies_warehouses_WarehouseID",
                        column: x => x.WarehouseID,
                        principalTable: "warehouses",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "warehouseStocks",
                columns: table => new
                {
                    WarehouseStockID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseID = table.Column<int>(type: "int", nullable: false),
                    MedicineID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouseStocks", x => x.WarehouseStockID);
                    table.ForeignKey(
                        name: "FK_warehouseStocks_medicines_MedicineID",
                        column: x => x.MedicineID,
                        principalTable: "medicines",
                        principalColumn: "MedicineID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_warehouseStocks_warehouses_WarehouseID",
                        column: x => x.WarehouseID,
                        principalTable: "warehouses",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "customerOrderDetails",
                columns: table => new
                {
                    CustomerOrderId = table.Column<int>(type: "int", nullable: false),
                    MedicineID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customerOrderDetails", x => new { x.CustomerOrderId, x.MedicineID });
                    table.ForeignKey(
                        name: "FK_customerOrderDetails_customerOrders_CustomerOrderId",
                        column: x => x.CustomerOrderId,
                        principalTable: "customerOrders",
                        principalColumn: "CustomerOrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_customerOrderDetails_medicines_MedicineID",
                        column: x => x.MedicineID,
                        principalTable: "medicines",
                        principalColumn: "MedicineID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerOrderID = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Method = table.Column<int>(type: "int", maxLength: 30, nullable: false),
                    Status = table.Column<int>(type: "int", maxLength: 30, nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefundReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RefundedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RefundedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK_payments_customerOrders_CustomerOrderID",
                        column: x => x.CustomerOrderID,
                        principalTable: "customerOrders",
                        principalColumn: "CustomerOrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PharmacistsOrder",
                columns: table => new
                {
                    PharmacistOrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PharmacistID = table.Column<int>(type: "int", nullable: false),
                    PharmacyID = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PharmacistsOrder", x => x.PharmacistOrderId);
                    table.ForeignKey(
                        name: "FK_PharmacistsOrder_pharmacies_PharmacyID",
                        column: x => x.PharmacyID,
                        principalTable: "pharmacies",
                        principalColumn: "PharmacyID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PharmacistsOrder_pharmacists_PharmacistID",
                        column: x => x.PharmacistID,
                        principalTable: "pharmacists",
                        principalColumn: "PharmacistID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PharmacistOrderDetails",
                columns: table => new
                {
                    PharmacistOrderId = table.Column<int>(type: "int", nullable: false),
                    MedicineID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PharmacistOrderDetails", x => new { x.PharmacistOrderId, x.MedicineID });
                    table.ForeignKey(
                        name: "FK_PharmacistOrderDetails_PharmacistsOrder_PharmacistOrderId",
                        column: x => x.PharmacistOrderId,
                        principalTable: "PharmacistsOrder",
                        principalColumn: "PharmacistOrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PharmacistOrderDetails_medicines_MedicineID",
                        column: x => x.MedicineID,
                        principalTable: "medicines",
                        principalColumn: "MedicineID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    TransferId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseID = table.Column<int>(type: "int", nullable: false),
                    PharmacyID = table.Column<int>(type: "int", nullable: false),
                    PharmacistOrderId = table.Column<int>(type: "int", nullable: false),
                    TransferDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.TransferId);
                    table.ForeignKey(
                        name: "FK_Transfers_PharmacistsOrder_PharmacistOrderId",
                        column: x => x.PharmacistOrderId,
                        principalTable: "PharmacistsOrder",
                        principalColumn: "PharmacistOrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfers_pharmacies_PharmacyID",
                        column: x => x.PharmacyID,
                        principalTable: "pharmacies",
                        principalColumn: "PharmacyID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfers_warehouses_WarehouseID",
                        column: x => x.WarehouseID,
                        principalTable: "warehouses",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transferDetails",
                columns: table => new
                {
                    TransferDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransferID = table.Column<int>(type: "int", nullable: false),
                    MedicineID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transferDetails", x => x.TransferDetailID);
                    table.ForeignKey(
                        name: "FK_transferDetails_Transfers_TransferID",
                        column: x => x.TransferID,
                        principalTable: "Transfers",
                        principalColumn: "TransferId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transferDetails_medicines_MedicineID",
                        column: x => x.MedicineID,
                        principalTable: "medicines",
                        principalColumn: "MedicineID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_customerOrderDetails_MedicineID",
                table: "customerOrderDetails",
                column: "MedicineID");

            migrationBuilder.CreateIndex(
                name: "IX_customerOrders_CustomerId",
                table: "customerOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_customerOrders_PharmacyId",
                table: "customerOrders",
                column: "PharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_customers_Email",
                table: "customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_customers_Phone",
                table: "customers",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_medicines_CategoryID",
                table: "medicines",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_payments_CustomerOrderID",
                table: "payments",
                column: "CustomerOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_pharmacies_Phone",
                table: "pharmacies",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PharmacistOrderDetails_MedicineID",
                table: "PharmacistOrderDetails",
                column: "MedicineID");

            migrationBuilder.CreateIndex(
                name: "IX_pharmacists_Email",
                table: "pharmacists",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pharmacists_PharmacyID",
                table: "pharmacists",
                column: "PharmacyID");

            migrationBuilder.CreateIndex(
                name: "IX_pharmacists_Phone",
                table: "pharmacists",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pharmacists_UserID",
                table: "pharmacists",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PharmacistsOrder_PharmacistID",
                table: "PharmacistsOrder",
                column: "PharmacistID");

            migrationBuilder.CreateIndex(
                name: "IX_PharmacistsOrder_PharmacyID",
                table: "PharmacistsOrder",
                column: "PharmacyID");

            migrationBuilder.CreateIndex(
                name: "IX_pharmacyStocks_MedicineID",
                table: "pharmacyStocks",
                column: "MedicineID");

            migrationBuilder.CreateIndex(
                name: "IX_pharmacyStocks_PharmacyID_MedicineID",
                table: "pharmacyStocks",
                columns: new[] { "PharmacyID", "MedicineID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_suppliers_Email_Phone",
                table: "suppliers",
                columns: new[] { "Email", "Phone" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Supplies_MedicineID",
                table: "Supplies",
                column: "MedicineID");

            migrationBuilder.CreateIndex(
                name: "IX_Supplies_SupplierID",
                table: "Supplies",
                column: "SupplierID");

            migrationBuilder.CreateIndex(
                name: "IX_Supplies_WarehouseID",
                table: "Supplies",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_transferDetails_MedicineID",
                table: "transferDetails",
                column: "MedicineID");

            migrationBuilder.CreateIndex(
                name: "IX_transferDetails_TransferID_MedicineID",
                table: "transferDetails",
                columns: new[] { "TransferID", "MedicineID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_PharmacistOrderId",
                table: "Transfers",
                column: "PharmacistOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_PharmacyID",
                table: "Transfers",
                column: "PharmacyID");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_WarehouseID",
                table: "Transfers",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_warehouseStocks_MedicineID",
                table: "warehouseStocks",
                column: "MedicineID");

            migrationBuilder.CreateIndex(
                name: "IX_warehouseStocks_WarehouseID_MedicineID",
                table: "warehouseStocks",
                columns: new[] { "WarehouseID", "MedicineID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customerOrderDetails");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "PharmacistOrderDetails");

            migrationBuilder.DropTable(
                name: "pharmacyStocks");

            migrationBuilder.DropTable(
                name: "Supplies");

            migrationBuilder.DropTable(
                name: "transferDetails");

            migrationBuilder.DropTable(
                name: "warehouseStocks");

            migrationBuilder.DropTable(
                name: "customerOrders");

            migrationBuilder.DropTable(
                name: "suppliers");

            migrationBuilder.DropTable(
                name: "Transfers");

            migrationBuilder.DropTable(
                name: "medicines");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "PharmacistsOrder");

            migrationBuilder.DropTable(
                name: "warehouses");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "pharmacists");

            migrationBuilder.DropTable(
                name: "pharmacies");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
