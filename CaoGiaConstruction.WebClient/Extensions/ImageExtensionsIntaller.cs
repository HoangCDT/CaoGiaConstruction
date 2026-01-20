using CaoGiaConstruction.WebClient.Extensions;
using Microsoft.AspNetCore.ResponseCompression;

namespace CaoGiaConstruction.WebClient.Installers
{
    public class ImageExtensionsIntaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            //Add service Middleware
            services.AddImageResizer();
            //Add MinResponseExtensions
            services.AddMinResponse();

            //Nén dung lượng response
            //Bât tính năng Response Compression Middleware cho các loại file trong MIME mặc định.
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes =
                    ResponseCompressionDefaults.MimeTypes.Concat(
                        new[] { "image/svg+xml" });
            });
        }
    }
}