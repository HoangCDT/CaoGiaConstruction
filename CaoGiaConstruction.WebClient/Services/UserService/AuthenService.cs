using Microsoft.AspNetCore.Identity;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.Utilities.Dtos;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Dtos;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IAuthenService
    {
        Task<OperationResult> LoginAsync(LoginDto model);

        Task<OperationResult> LogoutAsync();
    }

    public class AuthenService : IAuthenService, ITransientService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AppDbContext _context;

        public AuthenService(UserManager<User> userManager,
            AppDbContext context,
            SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<OperationResult> LoginAsync(LoginDto model)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    if (user.Status == AccountStatus.DeActive) //Trạng thái tạm Khóa
                    {
                        return new OperationResult(StatusCodes.Status400BadRequest, "Tài khoản đang bị khóa! Vui lòng liên hệ Administrator");
                    }
                    else if (user.Status == AccountStatus.Lock)
                    {
                        return new OperationResult(StatusCodes.Status400BadRequest, "Tài khoản bị vô hiệu hóa! Vui lòng liên hệ Administrator");
                    }
                    else
                    {
                        return new OperationResult(StatusCodes.Status200OK, "Đăng nhập thành công");
                    }
                }
                else
                {
                    return new OperationResult(StatusCodes.Status400BadRequest, "Tài khoản hoặc mật khẩu đúng! Vui lòng kiểm tra lại");
                }
            }
            catch (Exception ex)
            {
                return ex.GetMessageError();
            }
        }

        public async Task<OperationResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return new OperationResult(StatusCodes.Status200OK, "Đăng xuất thành công");
        }
    }
}