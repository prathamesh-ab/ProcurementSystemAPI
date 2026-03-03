
using ProcurementSystem.API.Data;
using ProcurementSystem.API.Interfaces;
using ProcurementSystem.API.Middleware;
using ProcurementSystem.API.Repositories;
using ProcurementSystem.API.Services;
using Serilog;
using System.Data;

namespace ProcurementSystem.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .WriteTo.Console()
                .WriteTo.File("logs/procurement-api-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting Procurement API");

                var builder = WebApplication.CreateBuilder(args);

                // Use Serilog
                builder.Host.UseSerilog();

                // Add services to the container.
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                // Add CORS
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAngular", policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
                });

                //Register dependencies
                builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
                builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
                builder.Services.AddScoped<ISupplierService, SupplierService>();

                var app = builder.Build();

                //Register the custom global exception middleware
                app.UseMiddleware<GlobalExceptionMiddleware>();

                app.UseSerilogRequestLogging();


                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();
                app.UseCors("AllowAngular");

                app.UseAuthorization();


                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
