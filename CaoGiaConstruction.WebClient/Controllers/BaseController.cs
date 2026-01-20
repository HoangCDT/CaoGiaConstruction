using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.Utilities.Constants;
using CaoGiaConstruction.WebClient.Extensions;
using static CaoGiaConstruction.Utilities.SetMetaTagUtility;

public class BaseClientController : Controller
{
    protected Metatag BuildMetaTag(string title = null, string siteName = null, string pageType = null,
        string description = null, string imageUrl = null, string keywords = null, string updateTime = null, string tag = null)
    {
        var currentUrlPath = Request.GetHostName();
        string defaultLogo = Commons.LOGO_TOP; // Sử dụng logo mặc định nếu không có

        // Giá trị mặc định nếu không có dữ liệu từ `about`
        string defaultDescription = "Cao Gia Construction - nơi mang đến cà phê rang xay nguyên chất, sáng tạo và chất lượng cao.";
        string defaultKeywords = "Cao Gia Construction, cà phê rang xay, cà phê nguyên chất, thưởng thức cà phê, cà phê chất lượng cao";
        string defaultTitle = "Cao Gia Construction";
        string defaultPageType = "article";

        return new Metatag
        {
            Title = !string.IsNullOrEmpty(title) ? title.Left(60, true, true) : defaultTitle,
            SiteName = !string.IsNullOrEmpty(siteName) ? siteName : defaultTitle, // Tên thương hiệu
            PageType = !string.IsNullOrEmpty(pageType) ? pageType : defaultPageType,
            Description = !string.IsNullOrEmpty(description) ? description.Left(158, true, true) : defaultDescription,
            Robots = "index,follow", // Robots chỉ mục
            Canonica = $"{currentUrlPath}{Request.Path}", // URL chính tắc
            Image = !string.IsNullOrEmpty(imageUrl)
                        ? $"{currentUrlPath}/{imageUrl}"
                        : $"{currentUrlPath}{defaultLogo}?w=600",// Ảnh đại diện bài viết (hoặc logo nếu không có)
            Locale = "vi_VN", // Ngôn ngữ
            Keywords = !string.IsNullOrEmpty(keywords) ? keywords.Left(70, true, true) : defaultKeywords, // Từ khóa SEO
            FBadmins = "", // Quản trị viên Facebook (nếu cần)
            UpdateTime = !string.IsNullOrEmpty(updateTime) ? updateTime : DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"), // Ngày cập nhật
            Tags = !string.IsNullOrEmpty(tag) ? tag.Left(70) : defaultKeywords // Tags liên quan
        };
    }
}
