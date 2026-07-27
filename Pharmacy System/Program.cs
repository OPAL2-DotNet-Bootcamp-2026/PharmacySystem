
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Pharmacy_System.Repos;
using Pharmacy_System.Services;
using Serilog;
using System.Text;

namespace Pharmacy_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("Logs/pharmacy-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Registers Serilog's services — required by UseSerilogRequestLogging().
            builder.Host.UseSerilog();

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddAuthorization();

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
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<EmailService>();


            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
                };
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });



            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Pharmacy System API",
                    Version = "v1"
                });

                // Adds the "Authorize" button
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Paste ONLY the token here — Swagger adds the word Bearer."
                });

                options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", doc),
                        new List<string>()
                    }
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Pharmacy System API v1");
                    options.RoutePrefix = "swagger";
                });
            }

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
