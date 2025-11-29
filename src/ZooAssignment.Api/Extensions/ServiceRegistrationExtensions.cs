using ZooAssignment.BusinessLayer.Mappings;
using ZooAssignment.BusinessLayer.Services;
using ZooAssignment.DataAccessLayer.Context;
using ZooAssignment.DataAccessLayer.FileReaders;
using ZooAssignment.DataAccessLayer.FileReaders.Factory;
using ZooAssignment.DataAccessLayer.Migrations;
using Microsoft.EntityFrameworkCore;

namespace ZooAssignment.Api.Extensions
{
    /// <summary>
    /// Extension methods for registering Zoo services
    /// This facade pattern encapsulates all service registration logic
    /// </summary>
    public static class ServiceRegistrationExtensions
    {
        /// <summary>
        /// Registers all Zoo-related services including database, AutoMapper, file readers, and business logic
        /// </summary>
        public static IServiceCollection AddZooServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Entity Framework Core with InMemory database
            services.AddDbContext<ZooContext>(options =>
                options.UseInMemoryDatabase("ZooDatabase"));

            // Add AutoMapper
            services.AddAutoMapper(mc =>
            {
                mc.AddProfile(new ZooMappingProfile());
            });

            // Add file readers
            services.AddScoped<AnimalTypesCsvReader>();
            services.AddScoped<ZooXmlReader>();
            services.AddScoped<FoodPriceTxtReader>();

            // Add custom services
            services.AddScoped<IFileReaderFactory, FileReaderFactory>();
            services.AddScoped<IMigrationService, MigrationService>();
            services.AddScoped<IZooService, ZooService>();

            return services;
        }

        /// <summary>
        /// Registers logging providers (Console and Debug)
        /// </summary>
        public static ILoggingBuilder AddZooLogging(this ILoggingBuilder logging)
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.AddDebug();
            logging.SetMinimumLevel(LogLevel.Information);

            return logging;
        }

        /// <summary>
        /// Runs database migrations on application startup
        /// </summary>
        public static async Task RunDatabaseMigrationsAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationService>();
                var basePath = app.Environment.ContentRootPath;

                logger.LogInformation("Starting database migration on application startup");
                try
                {
                    await migrationService.MigrateAsync(
                        Path.Combine(basePath, "animals.csv"),
                        Path.Combine(basePath, "prices.txt"),
                        Path.Combine(basePath, "zoo.xml")
                    );
                    logger.LogInformation("Database migration completed successfully");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Database migration failed on startup");
                    throw;
                }
            }
        }
    }
}
