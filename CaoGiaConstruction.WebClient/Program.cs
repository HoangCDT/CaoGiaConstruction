using CaoGiaConstruction.WebClient.Context;

namespace CaoGiaConstruction.WebClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Enable legacy timestamp behavior for PostgreSQL compatibility
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var dbInitializer = services.GetService<DbInitializer>();
                    dbInitializer.Seed().Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}