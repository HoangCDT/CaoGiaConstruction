namespace CaoGiaConstruction.Utilities
{
    public class SetMetaTagUtility
    {
        public static string ResolveDomainUrl(string url)
        {
            return url;
        }

        public static string SetMetaTags(Metatag meta)
        {
            if (meta == null)
            {
                throw new ArgumentNullException(nameof(meta));
            }

            //1. Thiết lập các meta Tag cho trang

            string metaTags = "<meta name='revisit-after' content='7 days'/>";

            if (!string.IsNullOrEmpty(meta.Title))
            {
                metaTags += "<title>" + meta.Title + "</title>";
                metaTags += "<meta itemprop='name' content='" + meta.Title + "'>";
                metaTags += "<meta property='og:title' content='" + meta.Title + "' /> ";
                metaTags += "<meta name='twitter:title' content ='" + meta.Title + "'/>";
                metaTags += "<meta name='twitter:card' content ='summary'/>";
            }
            if (!string.IsNullOrEmpty(meta.Description))
            {
                metaTags += "<meta name='description' content ='" + meta.Description + "'/>";
                metaTags += "<meta name='twitter:description' content ='" + meta.Description + "'/>";
                metaTags += "<meta itemprop='description' content='" + meta.Description + "' />";
                metaTags += "<meta property='og:description' content='" + meta.Description + "' /> ";
            }
            if (!string.IsNullOrEmpty(meta.Image))
            {
                metaTags += "<meta itemprop='image' content='" + ResolveDomainUrl(meta.Image) + "'>";
                metaTags += "<meta property='og:image' content='" + ResolveDomainUrl(meta.Image) + "'/>";
                metaTags += "<meta name='twitter:image' content='" + ResolveDomainUrl(meta.Image) + "'/>";
            }
            if (!string.IsNullOrEmpty(meta.Locale))
            {
                metaTags += "<meta property='og:locale' content='" + meta.Locale + "' />";
            }
            if (!string.IsNullOrEmpty(meta.PageType))
            {
                metaTags += "<meta property='og:type' content='" + meta.PageType + "' />";
            }
            if (!string.IsNullOrEmpty(meta.Canonica))
            {
                metaTags += "<meta property='og:url' content='" + meta.Canonica + "' />";
                metaTags += "<link rel='canonical' href='" + meta.Canonica + "'/>";
            }
            if (!string.IsNullOrEmpty(meta.Keywords))
            {
                metaTags += "<meta name='keywords' content='" + meta.Keywords + "'/>";
            }
            if (!string.IsNullOrEmpty(meta.GoogleAuthor))
            {
                metaTags += "<link rel='author' href='" + meta.GoogleAuthor + "'/>";
            }
            if (!string.IsNullOrEmpty(meta.SiteName))
            {
                metaTags += "<meta property='og:site_name' content='" + meta.SiteName + "' />";
                metaTags += "<meta name='twitter:site' content ='" + meta.SiteName + "'/>";
            }
            if (!string.IsNullOrEmpty(meta.GooglePublisher))
            {
                metaTags += "<link rel='publisher' href='" + meta.GooglePublisher + "' />";
            }
            if (!string.IsNullOrEmpty(meta.Robots))
            {
                metaTags += "<meta name='robots' content='" + meta.Robots + "'/>";
            }

            if (!string.IsNullOrEmpty(meta.PublishedTime))
            {
                metaTags += "<meta property='article:published_time' content='" + meta.PublishedTime + "' />";
            }

            if (!string.IsNullOrEmpty(meta.UpdateTime))
            {
                metaTags += "<meta property='article:modified_time' content='" + meta.UpdateTime + "' /> ";
            }

            if (!string.IsNullOrEmpty(meta.Section))
            {
                metaTags += "<meta property='article:section' content='" + meta.Section + "' />";
            }

            if (!string.IsNullOrEmpty(meta.Tags))
            {
                foreach (var tag in meta.Tags.Split(',').Select(t => t.Trim()))
                {
                    metaTags += "<meta property='article:tag' content='" + tag + "' />";
                }
            }
            if (meta.FBadmins != null)
            {
                metaTags += "<meta property='fb:admins' content='" + meta.FBadmins + "' />";
            }
            return metaTags;
        }

        public class Metatag
        {
            /// <summary>
            /// Tiêu đề trang
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Mô tả trang
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Từ khóa
            /// </summary>
            public string Keywords { get; set; }

            /// <summary>
            /// Config index cho trang
            /// </summary>
            public string Robots { get; set; }

            /// <summary>
            /// Mã chứng thực site với google
            /// </summary>
            public string GoogleSiteVerification { get; set; }

            /// <summary>
            /// Link trang hiện tại
            /// </summary>
            public string Canonica { get; set; }

            /// <summary>
            /// Tác giả google
            /// </summary>
            public string GoogleAuthor { get; set; }

            /// <summary>
            /// Người đăng google+
            /// </summary>
            public string GooglePublisher { get; set; }

            /// <summary>
            /// Link đến hình đại diện cho bài viết
            /// </summary>
            public string Image { get; set; }

            /// <summary>
            /// Mã ngôn ngữ site
            /// </summary>
            public string Locale { get; set; }

            /// <summary>
            /// Loại trang
            /// </summary>
            public string PageType { get; set; }

            /// <summary>
            /// Tên site
            /// </summary>
            public string SiteName { get; set; }

            /// <summary>
            /// Thời gian cập nhật bài viết
            /// </summary>
            public string UpdateTime { get; set; }

            /// <summary>
            /// Thời gian đăng bài viết
            /// </summary>
            public string PublishedTime { get; set; }

            /// <summary>
            /// Tên chuyên mục của bài viết
            /// </summary>
            public string Section { get; set; }

            /// <summary>
            /// Tên tag của bài viết, nếu nhiều tag phân cách chúng bằng dấu ","
            /// </summary>
            public string Tags { get; set; }

            /// <summary>
            /// Facebook Admin ID trang
            /// </summary>
            public string FBadmins { get; set; }

            /// <summary>
            /// Mã chứng thực với Alex
            /// </summary>
            public string AlexaSiteVerification { get; set; }
        }
    }
}