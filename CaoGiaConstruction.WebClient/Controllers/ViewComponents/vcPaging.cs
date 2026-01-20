using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.Utilities;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcPaging : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public vcPaging(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync(object pager)
        {
            var json = pager.ToJsonString();
            var dataPager = json.ToJsonObject<Pager<object>>();
            string url = GetCurrentUrl();
            string pageFirstUrl = url.Replace("XX", dataPager.PageFirst.ToString());
            string pageLastUrl = url.Replace("XX", dataPager.PageLast.ToString());
            string pageBackUrl = url.Replace("XX", dataPager.PagePrev.ToString());
            string pageNextUrl = url.Replace("XX", dataPager.PageNext.ToString());
            ViewBag.PageFirstUrl = pageFirstUrl;
            ViewBag.PageLastUrl = pageLastUrl;
            ViewBag.PageBackUrl = pageBackUrl;
            ViewBag.PageNextUrl = pageNextUrl;
            ViewBag.CurrentPage = dataPager.CurrentPage;
            ViewBag.TotalPage = dataPager.TotalPages;
            ViewBag.PageSize = dataPager.PageSize;
            ViewBag.TotalItems = dataPager.TotalItems;

            Dictionary<int, string> pageNumbers = new Dictionary<int, string>();
            foreach (var i in dataPager.Pages)
            {
                pageNumbers.Add(i, url.Replace("XX", i.ToString()));
            }

            return await Task.FromResult(View(pageNumbers));
        }

        private string GetCurrentUrl()
        {
            var currentUrl = _httpContextAccessor.HttpContext.Request.GetDisplayUrl();
            var uriBuilder = new UriBuilder(currentUrl);
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            query.Set("page", "XX");
            uriBuilder.Query = query.ToString();
            var modifiedUrl = uriBuilder.ToString();
            return modifiedUrl;
        }
    }
}