using System.Text.RegularExpressions;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.Utilities.Constants;
using CaoGiaConstruction.Utilities.Dtos;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Installers;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IFileService
    {
        Task<OperationResult> UploadFileAsync(IFormFile file, string pathFolder);

        Task<OperationResult> UploadFileMultipleAsync(List<IFormFile> files, string pathFolder);

        Task<OperationResult> UploadImageWithExtensionWebpAsync(IFormFile file, string pathFolder);

        Task<(byte[], string, string)> DownloadFileAsync(string fileUrl);

        Task<OperationResult> DeleteFileAsync(string fileUrl);

        Task<OperationResult> DeleteMultipleFileAsync(string fileList);
    }

    public class FileService : IFileService, ITransientService
    {
        private readonly IHostingEnvironment _env;

        public FileService(IHostingEnvironment env)
        {
            _env = env;
        }

        public async Task<OperationResult> UploadFileAsync(IFormFile file, string pathFolder)
        {
            var result = new OperationResult();

            if (file.Length > 0)
            {
                if (pathFolder.IsNullOrEmptyOrWhileSpace())
                {
                    pathFolder = $"{Commons.FILE_UPLOAD}/commons/";
                }

                //Khởi tạo folder
                var webRoot = _env.WebRootPath;
                bool exists = Directory.Exists(Path.Combine(webRoot, pathFolder + $"/"));

                if (!exists)
                    Directory.CreateDirectory(Path.Combine(webRoot, pathFolder + $"/"));

                var nowDate = DateTime.Now;
                string fileExtension = Path.GetExtension(file.FileName);
                string fileNewName = $"{Guid.NewGuid()}-{nowDate.Day}-{nowDate.Month}-{nowDate.Year}-{nowDate.Ticks.ToString().Left(5)}{fileExtension}";
                fileNewName = pathFolder + fileNewName;
                using (FileStream stream = File.Create($"wwwroot/" + fileNewName))
                {
                    try
                    {
                        await file.CopyToAsync(stream);
                        result = new OperationResult()
                        {
                            Success = true,
                            StatusCode = StatusCodes.Status200OK,
                            Data = fileNewName,
                            Message = MessageReponse.ADD_SUCCESS
                        };
                    }
                    catch (Exception ex)
                    {
                        result = ex.GetMessageError();
                    }
                }
            }
            else
            {
                result = new OperationResult()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null,
                    Message = MessageReponse.ADD_ERROR
                };
            }
            return await Task.FromResult(result);
        }

        public async Task<OperationResult> UploadFileMultipleAsync(List<IFormFile> files, string pathFolder)
        {
            List<string> result = new List<string>();
            foreach (var item in files)
            {
                var fileResult = await UploadImageWithExtensionWebpAsync(item, pathFolder);
                if (fileResult.Success)
                {
                    result.Add(fileResult.Data.ToString());
                }
            }
            return new OperationResult(StatusCodes.Status200OK, "Upload file thành công", String.Join(";", result));
        }

        public async Task<OperationResult> UploadImageWithExtensionWebpAsync(IFormFile file, string pathFolder)
        {
            var result = new OperationResult();

            if (file.Length > 0)
            {
                if (pathFolder.IsNullOrEmptyOrWhileSpace())
                {
                    pathFolder = $"{Commons.FILE_UPLOAD}/commons/";
                }

                //Khởi tạo folder
                var webRoot = _env.WebRootPath;
                bool exists = Directory.Exists(Path.Combine(webRoot, pathFolder + $"/"));

                if (!exists)
                    Directory.CreateDirectory(Path.Combine(webRoot, pathFolder + $"/"));

                var nowDate = DateTime.Now;
                string fileName = Path.GetFileNameWithoutExtension(file.FileName).ToFileFormat().Left(40);
                string baseFileNameConvert = $"{fileName}_{nowDate.Day}_{nowDate.Month}_{nowDate.Year}_{nowDate.Ticks}.webp";
                string outputPath = Path.Combine(webRoot, pathFolder, baseFileNameConvert);

                try
                {
                    // Đọc file ảnh gốc vào bộ nhớ
                    using (var inputStream = file.OpenReadStream())
                    using (var originalImage = SkiaSharp.SKBitmap.Decode(inputStream))
                    {
                        // Tạo ảnh mới với nền trắng
                        var whiteBackground = new SkiaSharp.SKBitmap(originalImage.Width, originalImage.Height);
                        using (var canvas = new SkiaSharp.SKCanvas(whiteBackground))
                        {
                            canvas.Clear(SkiaSharp.SKColors.White); // Đặt nền trắng
                            canvas.DrawBitmap(originalImage, 0, 0); // Vẽ ảnh gốc lên nền trắng
                        }

                        // Lưu ảnh dưới dạng WebP
                        using (var image = SkiaSharp.SKImage.FromBitmap(whiteBackground))
                        using (var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Webp, 100)) // Chất lượng 100%
                        {
                            await using (var fileStream = File.Create(outputPath))
                            {
                                data.SaveTo(fileStream);
                            }
                        }
                    }

                    result = new OperationResult()
                    {
                        Success = true,
                        StatusCode = StatusCodes.Status200OK,
                        Data = $"{pathFolder}{baseFileNameConvert}",
                        Message = MessageReponse.ADD_SUCCESS
                    };
                }
                catch (Exception ex)
                {
                    result = ex.GetMessageError();
                }
            }
            else
            {
                result = new OperationResult()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null,
                    Message = MessageReponse.ADD_ERROR
                };
            }

            return result;
        }

        public async Task<(byte[], string, string)> DownloadFileAsync(string fileUrl)
        {
            var url = Regex.Replace(fileUrl, @"\../", "");//CHống hack download file từ đường dẫn tương đối

            var pathFile = Path.Combine(_env.WebRootPath, url);
            if (File.Exists(pathFile))
            {
                var net = new System.Net.WebClient();
                var data = net.DownloadData(pathFile);

                var content = new MemoryStream(data);
                var contentType = "APPLICATION/octet-stream";

                var fileName = Path.GetFileName(url);
                return await Task.FromResult((content.ToArray(), contentType, fileName));
            }
            else
            {
                return (null, string.Empty, string.Empty);
            }
        }

        public async Task<OperationResult> DeleteFileAsync(string fileUrl)
        {
            var result = new OperationResult();

            if (fileUrl.IsNullOrEmpty())
            {
                return new OperationResult()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null,
                    Message = MessageReponse.DELETE_ERROR
                };
            }
            var url = Regex.Replace(fileUrl, @"\../", "");//Cống hack download file từ đường dẫn tương đối

            var pathFile = Path.Combine(_env.WebRootPath, url);
            if (File.Exists(pathFile))
            {
                try
                {
                    File.Delete(pathFile);
                    result = new OperationResult()
                    {
                        Success = true,
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Xoá File thành công"
                    };
                }
                catch (Exception ex)
                {
                    result = ex.GetMessageError();
                }
            }
            else
            {
                result = new OperationResult()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null,
                    Message = MessageReponse.ADD_ERROR
                };
            }
            return await Task.FromResult(result);
        }

        public async Task<OperationResult> DeleteMultipleFileAsync(string fileList)
        {
            if (fileList == null)
            {
                return new OperationResult(StatusCodes.Status200OK, MessageReponse.DELETE_SUCCESS);
            }
            var fileDeletes = fileList.Split(";").ToList();
            if (fileDeletes.Count() > 0)
            {
                foreach (var filePath in fileDeletes)
                {
                    await DeleteFileAsync(filePath);
                }
            }

            return new OperationResult(StatusCodes.Status200OK, MessageReponse.DELETE_SUCCESS);
        }
    }
}