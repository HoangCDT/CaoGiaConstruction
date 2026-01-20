namespace CaoGiaConstruction.Utilities
{
    public static class ImageExtensions
    {
        private const string DefaultImage = "/Admin/assets/images/no_image.png";
        private const string DefaultUserAvatar = "/Admin/assets/images/no_avatar.png";

        // Ensures the image path starts with a leading slash
        public static string ToHostImage(this object value, bool isSplit = false)
        {
            var stringValue = value?.ToString().ToSafetyString();
            if (string.IsNullOrEmpty(stringValue))
            {
                return DefaultImage;
            }

            if (!stringValue.Contains("http"))
            {
                stringValue = isSplit
                    ? ProcessImagePath(stringValue)
                    : EnsureLeadingSlash(stringValue);
            }

            return stringValue;
        }
        public static string ToHostUserAvatar(this object value, bool isSplit = false)
        {
            var stringValue = value?.ToString().ToSafetyString();
            if (string.IsNullOrEmpty(stringValue))
            {
                return DefaultUserAvatar;
            }

            if (!stringValue.Contains("http"))
            {
                stringValue = isSplit
                    ? ProcessImagePath(stringValue)
                    : EnsureLeadingSlash(stringValue);
            }

            return stringValue;
        }

        // Helper method to process image path when split is required
        private static string ProcessImagePath(string path)
        {
            var parts = path.Split(";");
            var firstPart = parts[0].ToSafetyString();
            return EnsureLeadingSlash(firstPart);
        }

        // Helper method to ensure the image path starts with a leading slash
        private static string EnsureLeadingSlash(string path)
        {
            return path.StartsWith("/") ? path : "/" + path;
        }
    }
}
