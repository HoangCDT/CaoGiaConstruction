using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;

namespace CaoGiaConstruction.WebClient.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var connetionString = Environment.GetEnvironmentVariable("CONNECT_STRING_API");

            if (string.IsNullOrEmpty(connetionString))
            {
                connetionString = configuration.GetConnectionString("Default") ?? configuration.GetConnectionString("DefaultConnection");
            }
            services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connetionString, o =>
           {
               o.EnableRetryOnFailure();
           }));

            services.AddIdentity<User, Role>()
                 .AddEntityFrameworkStores<AppDbContext>()
                 .AddDefaultTokenProviders();

            services.AddTransient<DbInitializer>();
        }
    }
}