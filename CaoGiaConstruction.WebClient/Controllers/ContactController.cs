using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.Utilities.Constants;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Extensions;
using CaoGiaConstruction.WebClient.Services;
using static CaoGiaConstruction.Utilities.SetMetaTagUtility;

namespace CaoGiaConstruction.WebClient.Controllers
{
    public class ContactController : BaseClientController
    {
        private readonly IAboutService _aboutService;
        private readonly IContactService _contactService;

        public ContactController(IAboutService aboutService, IContactService contactService)
        {
            _aboutService = aboutService;
            _contactService = contactService;
        }

        [Route("/lien-he", Name = "contact")]
        public async Task<IActionResult> Contact()
        {
            var about = await _aboutService.GetAboutCacheAsync();

            #region Seo Meta Tag
            var metaTag = BuildMetaTag(
               title: "Liên hệ Cao Gia Construction - Kết nối với chúng tôi", // Title (Thêm tiêu đề trang chứa từ khóa chính)
               siteName: "Cao Gia Construction", // SiteName (Tên trang web hoặc công ty)
               pageType: "contact", // PageType (Loại trang: product, article)
               description: "Liên hệ với Cao Gia Construction để được tư vấn và hỗ trợ về sản phẩm, dịch vụ. Địa chỉ, số điện thoại và email hỗ trợ luôn sẵn sàng phục vụ bạn.", // Description
               about.LogoTop, // Logo (Ảnh đại diện trang web)
               keywords: "liên hệ Cao Gia Construction, hỗ trợ khách hàng, địa chỉ Cao Gia Construction, số điện thoại Cao Gia Construction", // Keywords,
               updateTime: null, // UpdateTime
               tag: "liên hệ Cao Gia Construction, hỗ trợ khách hàng, địa chỉ Cao Gia Construction, số điện thoại Cao Gia Construction" // Tag (Các thẻ liên quan)
            );

            ViewBag.Header = SetMetaTags(metaTag);
            #endregion

            return View(about);
        }

        [HttpPost]
        [Route("post-contact")]
        public async Task<JsonResult> Contact(Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = MessageReponse.INVALID_DATA });
            }

            contact.Status = StatusEnum.InActive;

            var result = await _contactService.AddAsync(contact);

            return Json(result);
        }
    }
}