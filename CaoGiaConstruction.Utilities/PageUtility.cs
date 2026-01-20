using Microsoft.EntityFrameworkCore;
using CaoGiaConstruction.Utilities.Constants;

namespace CaoGiaConstruction.Utilities
{
    public static class PageUtility
    {
        public static async Task<Pager<T>> ToPaginationAsync<T>(this IQueryable<T> query, BasePagination param)
        {
            //Kiểm tra page có  -
            if (param.Page <= 0)
            {
                param.Page = 1;
            }

            //Tính tổng số lượng record
            var count = await query.CountAsync();

            //Tính số lượng bỏ qua
            int skip = (param.Page - 1) * param.PageSize;

            //Lấy ra số lượng record của trang hiện tại
            var items = await query.Skip(skip).Take(param.PageSize).ToListAsync();

            var totalPages = (int)Math.Ceiling((decimal)count / (decimal)param.PageSize);

            if (param.Page > totalPages)
            {
                param.Page = totalPages;
            }

            int startPage, endPage;
            int maxPages = Commons.MAX_PAGE_PAGINATION;
            if (totalPages <= maxPages)
            {
                startPage = 1;
                endPage = totalPages;
            }
            else
            {
                var maxPagesBeforeCurrentPage = (int)Math.Floor((decimal)maxPages / (decimal)2);
                var maxPagesAfterCurrentPage = (int)Math.Ceiling((decimal)maxPages / (decimal)2) - 1;
                if (param.Page <= maxPagesBeforeCurrentPage)
                {
                    startPage = 1;
                    endPage = maxPages;
                }
                else if (param.Page + maxPagesAfterCurrentPage >= totalPages)
                {
                    startPage = totalPages - maxPages + 1;
                    endPage = totalPages;
                }
                else
                {
                    startPage = param.Page - maxPagesBeforeCurrentPage;
                    endPage = param.Page + maxPagesAfterCurrentPage;
                }
            }

            var startIndex = (param.Page - 1) * param.PageSize;
            var endIndex = Math.Min(startIndex + param.PageSize - 1, count - 1);

            var pages = Enumerable.Range(startPage, (endPage + 1) - startPage).ToList();

            var page = new Pager<T>();
            page.Result = items;
            page.TotalItems = count;
            page.CurrentPage = param.Page;
            page.PageSize = param.PageSize;
            page.TotalPages = totalPages;
            page.StartPage = startPage;
            page.EndPage = endPage;
            page.StartIndex = startIndex;
            page.EndIndex = endIndex;
            page.PageNext = param.Page + 1 > totalPages ? totalPages : param.Page + 1;
            page.PagePrev = param.Page - 1 <= 0 ? 1 : param.Page - 1;
            page.PageFirst = 1;
            page.PageLast = totalPages;
            page.Pages = pages;

            return page;
        }
    }

    public class Pager<T>
    {
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int PageNext { get; set; }
        public int PagePrev { get; set; }
        public int PageFirst { get; set; }
        public int PageLast { get; set; }
        public List<int> Pages { get; set; }
        public List<T> Result { get; set; }
    }

    public class BasePagination
    {
        public virtual int Page { get; set; } = 1;
        public virtual int PageSize { get; set; } = 10;
    }
}