using Microsoft.AspNetCore.Http;

namespace CaoGiaConstruction.WebClient.Middleware
{
    /// <summary>
    /// Middleware to handle 404 errors for images and serve default image
    /// </summary>
    public class ImageNotFoundMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ImageNotFoundMiddleware> _logger;

        private static readonly string[] imageExtensions = new string[] {
            ".png",
            ".jpg",
            ".jpeg",
            ".webp",
            ".gif",
            ".bmp"
        };

        private const string DefaultImagePath = "/Admin/assets/images/no_image.png";

        public ImageNotFoundMiddleware(RequestDelegate next, IWebHostEnvironment env, ILogger<ImageNotFoundMiddleware> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if this is an image request before proceeding
            if (!IsImagePath(context.Request.Path))
            {
                await _next(context);
                return;
            }

            // Create a wrapper to capture the response status code
            var originalBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                // Check if we got a 404 for an image
                if (context.Response.StatusCode == 404)
                {
                    var defaultImageFullPath = Path.Combine(_env.WebRootPath, "Admin", "assets", "images", "no_image.png");
                    
                    if (File.Exists(defaultImageFullPath))
                    {
                        // Clear the 404 response
                        context.Response.Clear();
                        context.Response.StatusCode = 200;
                        
                        // Set content type based on original request or default to PNG
                        var extension = Path.GetExtension(context.Request.Path.Value).ToLowerInvariant();
                        if (extension == ".jpg" || extension == ".jpeg")
                        {
                            context.Response.ContentType = "image/jpeg";
                        }
                        else if (extension == ".webp")
                        {
                            context.Response.ContentType = "image/webp";
                        }
                        else
                        {
                            context.Response.ContentType = "image/png";
                        }
                        
                        // Set cache headers
                        context.Response.Headers["Cache-Control"] = "public, max-age=31536000, immutable";
                        
                        // Read and serve default image
                        var imageBytes = await File.ReadAllBytesAsync(defaultImageFullPath);
                        context.Response.ContentLength = imageBytes.Length;
                        
                        // Write to the original response stream
                        await originalBodyStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                        return;
                    }
                }

                // If not 404 or default image doesn't exist, copy the response
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private bool IsImagePath(PathString path)
        {
            if (path == null || !path.HasValue)
                return false;

            var pathValue = path.Value.ToLowerInvariant();
            return imageExtensions.Any(ext => pathValue.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }
    }
}

