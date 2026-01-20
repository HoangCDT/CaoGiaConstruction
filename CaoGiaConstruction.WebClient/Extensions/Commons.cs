using Microsoft.AspNetCore.Http.Extensions;
using CaoGiaConstruction.Utilities;

namespace CaoGiaConstruction.WebClient.Extensions
{
    public static class CommonApp
    {
        public static string ActiveMenu(this HttpRequest request, string path, bool removeExtent = false, bool isCompareHash = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            // Optionally remove file extension
            if (removeExtent)
            {
                path = path.Replace(".htm", string.Empty);
            }

            // Compare either by hash or by checking if the path is in the current URL
            return isCompareHash
                ? (path == request.Path ? "active" : string.Empty)
                : (request.GetDisplayUrl().Contains(path) ? "active" : string.Empty);
        }

    }
}