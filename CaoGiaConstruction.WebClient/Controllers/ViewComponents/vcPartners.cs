using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Const;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcPartners : ViewComponent
    {
        private readonly AppDbContext _context;

        public vcPartners(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var partners = await _context.Slides
                .Include(x => x.SlideCategory)
                .Where(x => x.Status == StatusEnum.Active 
                    && x.IsDeleted != true
                    && (x.SlideCategory.Code == SlideCategoryCodeDefine.HOME_SLIDE_PARTNER 
                        || x.SlideCategory.Title.Contains("Đối tác")))
                .OrderBy(x => x.SortOrder)
                .AsNoTracking()
                .ToListAsync();

            return View(partners);
        }
    }
}
