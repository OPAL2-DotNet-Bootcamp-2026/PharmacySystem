
using Pharmacy_System.Repos;
using Pharmacy_System.Services;

namespace Pharmacy_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<PharmacyContext>();

            builder.Services.AddScoped<CategoryRepo>();
            builder.Services.AddScoped<CustomerOrderRepo>(); 
            builder.Services.AddScoped<CustomerRepo>();            
            builder.Services.AddScoped<MedicineRepo>();
            builder.Services.AddScoped<PaymentRepo>();
            builder.Services.AddScoped<PharmacistOrderRepo>();
            builder.Services.AddScoped<PharmacistRepo>(); 
            builder.Services.AddScoped<PharmacyRepo>();            
            builder.Services.AddScoped<PharmacyStockRepo>();
            builder.Services.AddScoped<SupplierRepo>();
            builder.Services.AddScoped<SupplyRepo>();
            builder.Services.AddScoped<TransferRepo>();
            builder.Services.AddScoped<UserRepo>();
            builder.Services.AddScoped<WarehouseRepo>();
            builder.Services.AddScoped<WarehouseStockRepo>();

            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<CustomerOrderService>(); 
            builder.Services.AddScoped<CustomerService>();            
            builder.Services.AddScoped<MedicineService>();
            builder.Services.AddScoped<PaymentService>();
            builder.Services.AddScoped<PharmacistOrderService>();
            builder.Services.AddScoped<PharmacistService>();
            builder.Services.AddScoped<PharmacyService>();
            builder.Services.AddScoped<PharmacyStockService>();
            builder.Services.AddScoped<SupplierService>();
            builder.Services.AddScoped<SupplyService>();
            builder.Services.AddScoped<TransferService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<WarehouseService>();
            builder.Services.AddScoped<WarehouseStockService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
