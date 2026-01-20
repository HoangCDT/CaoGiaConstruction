namespace CaoGiaConstruction.WebClient.Configurations
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public string Key { get; set; }
        public string Issuer { get; set; }
        public TimeSpan TokenLifetime { get; set; }
    }
}