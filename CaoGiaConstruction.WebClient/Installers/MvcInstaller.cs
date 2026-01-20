using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using CaoGiaConstruction.Utilities.Constants;
using CaoGiaConstruction.WebClient.AutoMapper;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Extensions;

namespace CaoGiaConstruction.WebClient.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            ////Add Auto Mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IMapper>(sp =>
            {
                return new Mapper(AutoMapperConfig.RegisterMappings());
            });
            services.AddSingleton(AutoMapperConfig.RegisterMappings());
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSession(options =>
            {
                // Thiết lập thời gian timeout cho session (ví dụ: 30 phút)
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                // Thiết lập tên cookie session
                options.Cookie.Name = "CaoGiaConstructionSession";
            });

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;

                // User settings
                options.User.RequireUniqueEmail = true;
            });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(Commons.MINUTES_TIMEOUT);
                options.LoginPath = "/admin/dang-nhap";
                options.AccessDeniedPath = "/admin/account/forbidden";
                options.SlidingExpiration = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.IsEssential = true;
            });

            services.AddTransient<IdentityErrorDescriber, CustomIdentityErrorDescriber>();
            services.AddScoped<IUserClaimsPrincipalFactory<User>, CustomClaimsPrincipalFactory>();
        }
    }
}