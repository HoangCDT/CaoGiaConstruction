namespace CaoGiaConstruction.Utilities.Dtos
{
    public class AuthenticationResult<T> where T : class
    {
        public int StatusCode { set; get; }
        public string Message { set; get; }
        public bool Success { set; get; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public T Data { set; get; }
        public IEnumerable<string> Errors { get; set; }
    }
}