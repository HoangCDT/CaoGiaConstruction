namespace CaoGiaConstruction.WebClient.Dtos
{
    public class RegisterRequestDto
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
    }
}