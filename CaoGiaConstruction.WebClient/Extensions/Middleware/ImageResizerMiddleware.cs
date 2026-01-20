using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System.Reflection;
using System.Text;

namespace CaoGiaConstruction.WebClient.Middleware
{
    public class ImageResizerMiddleware
    {
        private struct ResizeParams
        {
            public bool hasParams;
            public bool isAutoOptimized; // Flag to indicate auto-optimization for large files
            public int w;
            public int h;
            public bool autorotate;
            public int quality; // 0 - 100
            public string format; // png, jpg, jpeg, webp
            public string mode; // pad, max, crop, stretch

            public static string[] modes = new string[] { "pad", "max", "crop", "stretch" };

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append($"w: {w}, ");
                sb.Append($"h: {h}, ");
                sb.Append($"autorotate: {autorotate}, ");
                sb.Append($"quality: {quality}, ");
                sb.Append($"format: {format}, ");
                sb.Append($"mode: {mode}, ");
                sb.Append($"isAutoOptimized: {isAutoOptimized}");

                return sb.ToString();
            }
        }

        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ImageResizerMiddleware> _logger;

        private static readonly string[] suffixes = new string[] {
            ".png",
            ".jpg",
            ".jpeg",
            ".webp"
        };

        // Default quality for better compression (85 is a good balance)
        private const int DefaultQuality = 85;

        // Large file optimization settings
        private const long LargeFileThresholdBytes = 5 * 1024 * 1024; // 5MB
        private const double LargeFileResizeRatio = 0.5; // 50% of original size
        private const int LargeFileQuality = 50; // 50% quality for large files

        public ImageResizerMiddleware(RequestDelegate next, IWebHostEnvironment env, IMemoryCache memoryCache, ILogger<ImageResizerMiddleware> logger)
        {
            _next = next;
            _env = env;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path;

            // hand to next middleware if we are not dealing with an image
            if (!IsImagePath(path))
            {
                await _next.Invoke(context);
                return;
            }

            // get the image location on disk first to check file size
            var imagePath = Path.Combine(
                _env.WebRootPath,
                path.Value.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar)).Replace("//", "/");

            // Get file info for large file detection
            FileInfo fileInfo = null;
            bool fileExists = File.Exists(imagePath);
            if (fileExists)
            {
                fileInfo = new FileInfo(imagePath);
            }

            // Get resize params
            var resizeParams = GetResizeParams(path, context.Request.Query, context.Request.Headers, fileInfo);
            
            // Log file info for debugging
            if (fileExists && fileInfo != null)
            {
                _logger.LogInformation("Processing image: {Path}, Size: {Size}MB, HasParams: {HasParams}, W: {W}, H: {H}",
                    path.Value, 
                    Math.Round(fileInfo.Length / (1024.0 * 1024.0), 2),
                    resizeParams.hasParams,
                    resizeParams.w,
                    resizeParams.h);
            }
            
            // Auto-optimize large files (>5MB) even if no resize params provided
            if (fileExists && fileInfo != null && fileInfo.Length > LargeFileThresholdBytes)
            {
                // Force resize params for large files to enable optimization
                if (!resizeParams.hasParams || (resizeParams.w == 0 && resizeParams.h == 0))
                {
                    resizeParams.hasParams = true;
                    resizeParams.isAutoOptimized = true; // Mark as auto-optimized
                    resizeParams.w = 0; // Will be set to 50% in GetImageData after loading bitmap
                    resizeParams.h = 0;
                    resizeParams.mode = "max";
                    
                    // Auto-convert to WebP if browser supports (better compression)
                    bool supportsWebP = false;
                    if (context.Request.Headers.ContainsKey("Accept"))
                    {
                        var acceptHeader = context.Request.Headers["Accept"].ToString();
                        supportsWebP = acceptHeader.Contains("image/webp", StringComparison.OrdinalIgnoreCase);
                    }
                    
                    if (supportsWebP)
                    {
                        resizeParams.format = "webp";
                    }
                    else
                    {
                        // Determine format from file extension
                        var extension = Path.GetExtension(path.Value).ToLowerInvariant();
                        if (extension == ".png")
                            resizeParams.format = "png";
                        else
                            resizeParams.format = "jpeg";
                    }
                    
                    // Set quality to 50% for large files if not explicitly set
                    if (!context.Request.Query.ContainsKey("quality"))
                    {
                        resizeParams.quality = LargeFileQuality;
                    }
                    
                    _logger.LogInformation("Auto-optimizing large file without resize params: {Path} ({Size}MB) - will resize to 50% and quality {Quality}%",
                        path.Value, Math.Round(fileInfo.Length / (1024.0 * 1024.0), 2), resizeParams.quality);
                }
                else
                {
                    // File has resize params but is large - ensure quality is optimized
                    if (!context.Request.Query.ContainsKey("quality") && resizeParams.quality > LargeFileQuality)
                    {
                        resizeParams.quality = LargeFileQuality;
                        _logger.LogInformation("Auto-reducing quality for large file with resize params: {Path} ({Size}MB) to {Quality}%",
                            path.Value, Math.Round(fileInfo.Length / (1024.0 * 1024.0), 2), resizeParams.quality);
                    }
                }
            }
            
            // hand to next middleware if we are dealing with an image but it doesn't have any usable resize querystring params
            // Exception: if it's auto-optimized (large file), continue processing even with w=0
            if (!resizeParams.hasParams || (!resizeParams.isAutoOptimized && resizeParams.w == 0 && resizeParams.h == 0))
            {
                // File is not large or doesn't have resize params, proceed normally
                await _next.Invoke(context);
                return;
            }

            // if we got this far, resize it
            try
            {
                // check if file exists
                if (!File.Exists(imagePath))
                {
                    // If image doesn't exist, try to serve default image
                    var defaultImagePath = Path.Combine(_env.WebRootPath, "Admin", "assets", "images", "no_image.png");
                    if (File.Exists(defaultImagePath))
                    {
                        // Serve default image with same resize parameters
                        var defaultImageData = GetImageData(defaultImagePath, resizeParams, File.GetLastWriteTimeUtc(defaultImagePath));
                        
                        // Set response headers for caching
                        context.Response.Headers["Cache-Control"] = "public, max-age=31536000, immutable";
                        
                        // Determine the content type based on the requested format
                        if (resizeParams.format == "webp")
                        {
                            context.Response.ContentType = "image/webp";
                        }
                        else if (resizeParams.format == "png")
                        {
                            context.Response.ContentType = "image/png";
                        }
                        else
                        {
                            context.Response.ContentType = "image/jpeg";
                        }
                        
                        context.Response.ContentLength = defaultImageData.Size;
                        await context.Response.Body.WriteAsync(defaultImageData.ToArray(), 0, (int)defaultImageData.Size);
                        defaultImageData.Dispose();
                        return;
                    }
                    
                    // If default image also doesn't exist, pass to next middleware
                    await _next.Invoke(context);
                    return;
                }

                // check file lastwrite
                var lastWriteTimeUtc = File.GetLastWriteTimeUtc(imagePath);
                
                var imageData = GetImageData(imagePath, resizeParams, lastWriteTimeUtc);

                // Set response headers for caching
                context.Response.Headers["Cache-Control"] = "public, max-age=31536000, immutable";
                context.Response.Headers["ETag"] = $"\"{lastWriteTimeUtc.Ticks}\"";
                context.Response.Headers["Last-Modified"] = lastWriteTimeUtc.ToString("R");

                // Determine the content type based on the requested format
                if (resizeParams.format == "webp")
                {
                    context.Response.ContentType = "image/webp";
                }
                else if (resizeParams.format == "png")
                {
                    context.Response.ContentType = "image/png";
                }
                else
                {
                    context.Response.ContentType = "image/jpeg";
                }

                context.Response.ContentLength = imageData.Size;
                await context.Response.Body.WriteAsync(imageData.ToArray(), 0, (int)imageData.Size);

                // cleanup
                imageData.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resizing image: {Path}", path.Value);
                // If resize fails, pass to next middleware to serve original
                await _next.Invoke(context);
            }
        }

        private SKData GetImageData(string imagePath, ResizeParams resizeParams, DateTime lastWriteTimeUtc)
        {
            // check cache and return if cached
            long cacheKey;
            unchecked
            {
                cacheKey = imagePath.GetHashCode() + lastWriteTimeUtc.ToBinary() + resizeParams.ToString().GetHashCode();
            }

            SKData imageData;
            byte[] imageBytes;
            bool isCached = _memoryCache.TryGetValue<byte[]>(cacheKey, out imageBytes);
            if (isCached)
            {
                // _logger.LogInformation("Serving from cache");
                return SKData.CreateCopy(imageBytes);
            }

            SKEncodedOrigin origin; // this represents the EXIF orientation
            var bitmap = LoadBitmap(File.OpenRead(imagePath), out origin); // always load as 32bit (to overcome issues with indexed color)

            // Auto-resize large files (>5MB) to 50% of original size
            var fileInfoCheck = new FileInfo(imagePath);
            if (fileInfoCheck.Length > LargeFileThresholdBytes)
            {
                int originalWidth = bitmap.Width;
                int originalHeight = bitmap.Height;
                
                // Auto-resize to 50% if no explicit width or width is larger than 50% of original
                if (resizeParams.w == 0 || resizeParams.w > originalWidth * LargeFileResizeRatio)
                {
                    resizeParams.w = (int)Math.Round(originalWidth * LargeFileResizeRatio);
                    resizeParams.h = 0; // Auto-calculate height to maintain aspect ratio
                    resizeParams.mode = "max";
                    
                    _logger.LogInformation("Auto-resizing large image in GetImageData: {Path} ({Size}MB) from {OriginalWidth}x{OriginalHeight}px to {NewWidth}px (50%)",
                        imagePath, 
                        Math.Round(fileInfoCheck.Length / (1024.0 * 1024.0), 2),
                        originalWidth, 
                        originalHeight, 
                        resizeParams.w);
                }
                
                // Ensure quality is set to 50% for large files if not explicitly set
                if (resizeParams.quality > LargeFileQuality)
                {
                    resizeParams.quality = LargeFileQuality;
                    _logger.LogInformation("Auto-reducing quality in GetImageData for large file: {Path} to {Quality}%",
                        imagePath, resizeParams.quality);
                }
            }

            // if autorotate = true, and origin isn't correct for the rotation, rotate it
            if (resizeParams.autorotate && origin != SKEncodedOrigin.TopLeft)
                bitmap = RotateAndFlip(bitmap, origin);

            // if either w or h is 0, set it based on ratio of original image
            if (resizeParams.h == 0)
                resizeParams.h = (int)Math.Round(bitmap.Height * (float)resizeParams.w / bitmap.Width);
            else if (resizeParams.w == 0)
                resizeParams.w = (int)Math.Round(bitmap.Width * (float)resizeParams.h / bitmap.Height);

            // if we need to crop, crop the original before resizing
            if (resizeParams.mode == "crop")
                bitmap = Crop(bitmap, resizeParams);

            // store padded height and width
            var paddedHeight = resizeParams.h;
            var paddedWidth = resizeParams.w;

            // if we need to pad, or max, set the height or width according to ratio
            if (resizeParams.mode == "pad" || resizeParams.mode == "max")
            {
                var bitmapRatio = (float)bitmap.Width / bitmap.Height;
                var resizeRatio = (float)resizeParams.w / resizeParams.h;

                if (bitmapRatio > resizeRatio) // original is more "landscape"
                    resizeParams.h = (int)Math.Round(bitmap.Height * ((float)resizeParams.w / bitmap.Width));
                else
                    resizeParams.w = (int)Math.Round(bitmap.Width * ((float)resizeParams.h / bitmap.Height));
            }

            // resize
            var resizedImageInfo = new SKImageInfo(resizeParams.w, resizeParams.h, SKImageInfo.PlatformColorType, bitmap.AlphaType);
            var samplingOptions = new SKSamplingOptions(SKCubicResampler.Mitchell);
            var resizedBitmap = bitmap.Resize(resizedImageInfo, samplingOptions);

            // optionally pad
            if (resizeParams.mode == "pad")
                resizedBitmap = Pad(resizedBitmap, paddedWidth, paddedHeight, resizeParams.format != "png");

            // encode
            var resizedImage = SKImage.FromBitmap(resizedBitmap);
            SKEncodedImageFormat encodeFormat;
            if (resizeParams.format == "webp")
            {
                encodeFormat = SKEncodedImageFormat.Webp;
                // WebP encoding with quality
                imageData = resizedImage.Encode(encodeFormat, resizeParams.quality);
            }
            else if (resizeParams.format == "png")
            {
                encodeFormat = SKEncodedImageFormat.Png;
                // For PNG, SkiaSharp uses zlib compression level
                // PNG encoding - use quality parameter (0-100) where higher = better compression but slower
                // We'll use quality in the 80-95 range for optimal PNG compression
                // Higher compression = smaller file size
                int pngQuality = Math.Min(95, Math.Max(80, resizeParams.quality)); // Use 80-95 range for PNG compression
                imageData = resizedImage.Encode(encodeFormat, pngQuality);
            }
            else
            {
                encodeFormat = SKEncodedImageFormat.Jpeg;
                imageData = resizedImage.Encode(encodeFormat, resizeParams.quality);
            }

            // cache the result
            _memoryCache.Set<byte[]>(cacheKey, imageData.ToArray());

            // cleanup
            resizedImage.Dispose();
            bitmap.Dispose();
            resizedBitmap.Dispose();

            return imageData;
        }

        private SKBitmap RotateAndFlip(SKBitmap original, SKEncodedOrigin origin)
        {
            // these are the origins that represent a 90 degree turn in some fashion
            var differentOrientations = new SKEncodedOrigin[]
            {
                SKEncodedOrigin.LeftBottom,
                SKEncodedOrigin.LeftTop,
                SKEncodedOrigin.RightBottom,
                SKEncodedOrigin.RightTop
            };

            // check if we need to turn the image
            bool isDifferentOrientation = differentOrientations.Any(o => o == origin);

            // define new width/height
            var width = isDifferentOrientation ? original.Height : original.Width;
            var height = isDifferentOrientation ? original.Width : original.Height;

            var bitmap = new SKBitmap(width, height, original.AlphaType == SKAlphaType.Opaque);

            // todo: the stuff in this switch statement should be rewritten to use pointers
            switch (origin)
            {
                case SKEncodedOrigin.LeftBottom:

                    for (var x = 0; x < original.Width; x++)
                        for (var y = 0; y < original.Height; y++)
                            bitmap.SetPixel(y, original.Width - 1 - x, original.GetPixel(x, y));
                    break;

                case SKEncodedOrigin.RightTop:

                    for (var x = 0; x < original.Width; x++)
                        for (var y = 0; y < original.Height; y++)
                            bitmap.SetPixel(original.Height - 1 - y, x, original.GetPixel(x, y));
                    break;

                case SKEncodedOrigin.RightBottom:

                    for (var x = 0; x < original.Width; x++)
                        for (var y = 0; y < original.Height; y++)
                            bitmap.SetPixel(original.Height - 1 - y, original.Width - 1 - x, original.GetPixel(x, y));

                    break;

                case SKEncodedOrigin.LeftTop:

                    for (var x = 0; x < original.Width; x++)
                        for (var y = 0; y < original.Height; y++)
                            bitmap.SetPixel(y, x, original.GetPixel(x, y));
                    break;

                case SKEncodedOrigin.BottomLeft:

                    for (var x = 0; x < original.Width; x++)
                        for (var y = 0; y < original.Height; y++)
                            bitmap.SetPixel(x, original.Height - 1 - y, original.GetPixel(x, y));
                    break;

                case SKEncodedOrigin.BottomRight:

                    for (var x = 0; x < original.Width; x++)
                        for (var y = 0; y < original.Height; y++)
                            bitmap.SetPixel(original.Width - 1 - x, original.Height - 1 - y, original.GetPixel(x, y));
                    break;

                case SKEncodedOrigin.TopRight:

                    for (var x = 0; x < original.Width; x++)
                        for (var y = 0; y < original.Height; y++)
                            bitmap.SetPixel(original.Width - 1 - x, y, original.GetPixel(x, y));
                    break;
            }

            original.Dispose();

            return bitmap;
        }

        private SKBitmap LoadBitmap(Stream stream, out SKEncodedOrigin origin)
        {
            origin = SKEncodedOrigin.TopLeft; // Default origin
            if (stream == null || !stream.CanRead || stream.Length == 0)
            {
                throw new ArgumentException("The provided stream is null, unreadable, or empty.");
            }

            // Ensure the stream is at the beginning
            stream.Position = 0;

            try
            {
                using (var s = new SKManagedStream(stream))
                {
                    if (s == null)
                        return null;

                    var codec = SKCodec.Create(s);
                    if (codec == null)
                        return null;

                    origin = codec.EncodedOrigin;
                    var info = codec.Info;
                    var bitmap = new SKBitmap(info.Width, info.Height, SKImageInfo.PlatformColorType, info.IsOpaque ? SKAlphaType.Opaque : SKAlphaType.Premul);

                    IntPtr length;
                    var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels(out length));
                    if (result == SKCodecResult.Success || result == SKCodecResult.IncompleteInput)
                    {
                        return bitmap;
                    }
                    else
                    {
                        throw new ArgumentException("Unable to load bitmap from provided data");
                    }
                }
            }
            catch
            {
                // Load a default or fallback image if an error occurs
                origin = SKEncodedOrigin.TopLeft; // Default origin
                var fallbackBitmap = new SKBitmap(100, 100); // Example size for the fallback
                using (var canvas = new SKCanvas(fallbackBitmap))
                {
                    canvas.Clear(SKColors.Gray); // Fill fallback with a solid color
                }
                return fallbackBitmap;
            }

        }

        private SKBitmap Crop(SKBitmap original, ResizeParams resizeParams)
        {
            var cropSides = 0;
            var cropTopBottom = 0;

            // calculate amount of pixels to remove from sides and top/bottom
            if ((float)resizeParams.w / original.Width < resizeParams.h / original.Height) // crop sides
                cropSides = original.Width - (int)Math.Round((float)original.Height / resizeParams.h * resizeParams.w);
            else
                cropTopBottom = original.Height - (int)Math.Round((float)original.Width / resizeParams.w * resizeParams.h);

            // setup crop rect
            var cropRect = new SKRectI
            {
                Left = cropSides / 2,
                Top = cropTopBottom / 2,
                Right = original.Width - cropSides + cropSides / 2,
                Bottom = original.Height - cropTopBottom + cropTopBottom / 2
            };

            // crop
            SKBitmap bitmap = new SKBitmap(cropRect.Width, cropRect.Height);
            original.ExtractSubset(bitmap, cropRect);
            original.Dispose();

            return bitmap;
        }

        private SKBitmap Pad(SKBitmap original, int paddedWidth, int paddedHeight, bool isOpaque)
        {
            // setup new bitmap and optionally clear
            var bitmap = new SKBitmap(paddedWidth, paddedHeight, isOpaque);
            var canvas = new SKCanvas(bitmap);
            if (isOpaque)
                canvas.Clear(new SKColor(255, 255, 255)); // we could make this color a resizeParam
            else
                canvas.Clear(SKColor.Empty);

            // find co-ords to draw original at
            var left = original.Width < paddedWidth ? (paddedWidth - original.Width) / 2 : 0;
            var top = original.Height < paddedHeight ? (paddedHeight - original.Height) / 2 : 0;

            var drawRect = new SKRectI
            {
                Left = left,
                Top = top,
                Right = original.Width + left,
                Bottom = original.Height + top
            };

            // draw original onto padded version
            canvas.DrawBitmap(original, drawRect);
            canvas.Flush();

            canvas.Dispose();
            original.Dispose();

            return bitmap;
        }

        private bool IsImagePath(PathString path)
        {
            if (path == null || !path.HasValue)
                return false;

            var pathValue = path.Value.ToLowerInvariant();
            return suffixes.Any(suffix => pathValue.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
        }

        private ResizeParams GetResizeParams(PathString path, IQueryCollection query, IHeaderDictionary headers, FileInfo fileInfo = null)
        {
            ResizeParams resizeParams = new ResizeParams();

            // before we extract, do a quick check for resize params
            resizeParams.hasParams =
                resizeParams.GetType().GetTypeInfo()
                .GetFields().Where(f => f.Name != "hasParams")
                .Any(f => query.ContainsKey(f.Name));

            // if no params present, bug out
            if (!resizeParams.hasParams)
                return resizeParams;

            // extract resize params

            // Check if browser supports WebP
            bool supportsWebP = false;
            if (headers.ContainsKey("Accept"))
            {
                var acceptHeader = headers["Accept"].ToString();
                supportsWebP = acceptHeader.Contains("image/webp", StringComparison.OrdinalIgnoreCase);
            }

            // Determine format - prefer WebP if browser supports it and format not explicitly specified
            if (query.ContainsKey("format"))
            {
                resizeParams.format = query["format"].ToString().ToLowerInvariant();
            }
            else
            {
                // Auto-detect format from path
                var lastDotIndex = path.Value.LastIndexOf('.');
                if (lastDotIndex >= 0 && lastDotIndex < path.Value.Length - 1)
                {
                    var originalFormat = path.Value.Substring(lastDotIndex + 1).ToLowerInvariant();
                    
                    // For PNG files: convert to WebP if browser supports it (WebP supports transparency too)
                    // This can reduce PNG file size by 25-35% while maintaining quality
                    if (originalFormat == "png")
                    {
                        if (supportsWebP)
                        {
                            resizeParams.format = "webp"; // WebP supports transparency, so safe to convert
                        }
                        else
                        {
                            resizeParams.format = "png"; // Keep PNG if browser doesn't support WebP
                        }
                    }
                    else if (supportsWebP && originalFormat != "png")
                    {
                        // For other formats, convert to WebP if browser supports it
                        resizeParams.format = "webp";
                    }
                    else
                    {
                        resizeParams.format = originalFormat;
                    }
                }
                else
                {
                    // Default to WebP if browser supports it, otherwise jpeg
                    resizeParams.format = supportsWebP ? "webp" : "jpeg";
                }
            }

            if (query.ContainsKey("autorotate"))
                bool.TryParse(query["autorotate"], out resizeParams.autorotate);

            // Use default quality if not specified
            int quality = DefaultQuality;
            if (query.ContainsKey("quality"))
                int.TryParse(query["quality"], out quality);
            
            // Auto-optimize large files (>5MB): reduce quality to 50% if not explicitly set
            if (fileInfo != null && fileInfo.Length > LargeFileThresholdBytes && !query.ContainsKey("quality"))
            {
                quality = LargeFileQuality;
                _logger.LogInformation("Auto-reducing quality for large file: {Path} ({Size}MB) to {Quality}%",
                    path.Value, Math.Round(fileInfo.Length / (1024.0 * 1024.0), 2), quality);
            }
            
            // Clamp quality between 1 and 100
            resizeParams.quality = Math.Max(1, Math.Min(100, quality));
            
            // For WebP format, optimize quality for better compression (70-75% range)
            // WebP at 70-75% provides excellent compression while maintaining good visual quality
            // Skip this if quality was already set for large files
            if (resizeParams.format == "webp" && quality == DefaultQuality && (fileInfo == null || fileInfo.Length <= LargeFileThresholdBytes))
            {
                // Use 75% quality for WebP to maximize compression
                resizeParams.quality = 75; // Optimized compression for WebP
            }
            
            // For PNG format, optimize quality for better compression
            // PNG quality affects compression level: higher = better compression but slower
            // We'll use a range that balances file size and encoding speed
            // Skip this if quality was already set for large files
            if (resizeParams.format == "png" && quality == DefaultQuality && (fileInfo == null || fileInfo.Length <= LargeFileThresholdBytes))
            {
                // Use higher quality (compression level) for PNG to reduce file size
                resizeParams.quality = 90; // Higher compression for PNG
            }

            int w = 0;
            if (query.ContainsKey("w"))
                int.TryParse(query["w"], out w);
            resizeParams.w = w;

            int h = 0;
            if (query.ContainsKey("h"))
                int.TryParse(query["h"], out h);
            resizeParams.h = h;

            resizeParams.mode = "max";
            // only apply mode if it's a valid mode and both w and h are specified
            if (h != 0 && w != 0 && query.ContainsKey("mode") && ResizeParams.modes.Any(m => query["mode"] == m))
                resizeParams.mode = query["mode"];

            return resizeParams;
        }
    }
}