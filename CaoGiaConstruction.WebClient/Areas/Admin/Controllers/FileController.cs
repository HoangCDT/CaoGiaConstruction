using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class FileController : BaseController
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(string pathFolder)
        {
            var file = Request.Form.Files.LastOrDefault();
            var result = await _fileService.UploadImageWithExtensionWebpAsync(file, pathFolder);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UploadMultipleFile(string pathFolder)
        {
            var files = Request.Form.Files.ToList();
            var result = await _fileService.UploadFileMultipleAsync(files, pathFolder);
            return Ok(result);
        }
    }
}