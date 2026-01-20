using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Dtos;
using CaoGiaConstruction.WebClient.Services;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class AuthenController : BaseController
    {
        private readonly IAuthenService _authenService;

        public AuthenController(IAuthenService authenService)
        {
            _authenService = authenService;
        }

        [AllowAnonymous]
        [Route("/{area}/dang-nhap", Name = "admin-login")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("/{area}/dang-nhap", Name = "admin-login-post")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginDto request, string returnUrl = null)
        {
            var resultLogin = await _authenService.LoginAsync(request);
            if (resultLogin.Success)
            {
                return RedirectToRoute("admin-default");
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = resultLogin.Message;
                return View();
            }
        }

        [Route("/{area}/dang-xuat", Name = "admin-logout")]
        public async Task<IActionResult> Logout()
        {
            await _authenService.LogoutAsync();
            return View();
        }
    }
}