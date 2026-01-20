namespace CaoGiaConstruction.WebClient.Extensions
{
    public static class UrlExtendtion
    {
        public static string GetHostName(this HttpRequest request)
        {
            var currentUrlPath = $"https://{request.Host}";
            return currentUrlPath;
        }
    }
}