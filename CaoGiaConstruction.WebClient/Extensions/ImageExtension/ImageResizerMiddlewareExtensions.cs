using CaoGiaConstruction.WebClient.Middleware;

namespace CaoGiaConstruction.WebClient.Extensions
{
    public static class ImageResizerMiddlewareExtensions
    {
        public static IServiceCollection AddImageResizer(this IServiceCollection services)
        {
            return services.AddMemoryCache();
        }

        public static IApplicationBuilder UseImageResizer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ImageResizerMiddleware>();
        }

        public static IApplicationBuilder UseImageNotFoundHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ImageNotFoundMiddleware>();
        }
    }
}
