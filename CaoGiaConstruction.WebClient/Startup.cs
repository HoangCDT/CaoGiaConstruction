using Microsoft.Extensions.Options;
using CaoGiaConstruction.WebClient.Extensions;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.InstallServicesInAssembly(Configuration);
            services.AddHttpContextAccessor();
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor contextAccessor)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error/Index", "?code={0}");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseImageResizer();
            app.UseStaticFiles();
            app.UseImageNotFoundHandler(); // Handle 404 for images - must be after StaticFiles
            
            //MinResponseExtensions
            //app.UseMinResponse();

            app.UseRouting();
            app.UseSession();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            //Nén dung lượng response
            app.UseResponseCompression();
            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                OnPrepareResponse = context => context.Context.Response.Headers.Append("Cache-Control", "public, max-age=2592000")
            });


            //Setup file robots
            app.Map("/robots.txt", builder =>
            {
                builder.Run(async context =>
                {
                    var hostName = $"{context.Request.Scheme}://{context.Request.Host}"; // Dynamically get the hostname
                    var robotsTxtContent = $"User-agent: *\r\n" +
                                           $"Disallow: \r\n" +
                                           $"Disallow: /cgi-bin/\r\n" +
                                           $"Sitemap: {hostName}/sitemap.xml";

                    context.Response.ContentType = "text/plain"; // Ensure the correct content type
                    await context.Response.WriteAsync(robotsTxtContent);
                });
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                   name: "admin",
                   areaName: "admin",
                pattern: "admin/{controller=home}/{action=index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}