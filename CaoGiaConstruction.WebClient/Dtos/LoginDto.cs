using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Trường tên đăng nhập là bắt buộc.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Trường mật khẩu là bắt buộc.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}