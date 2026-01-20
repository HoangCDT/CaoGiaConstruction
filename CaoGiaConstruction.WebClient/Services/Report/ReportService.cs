using Microsoft.EntityFrameworkCore;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Dtos;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IReportService
    {
        Task<ReportHomeDto> GetCountSideBar();

        Task<ReportHomeDto> GetStatisticalHome();
    }

    public class ReportService : IReportService, ITransientService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ReportHomeDto> GetStatisticalHome()
        {
            var numberProduct = await _context.Products.CountAsync();
            var numberBlog = await _context.Blogs.CountAsync();
            var numberContact = await _context.Contacts.CountAsync();
            var result = new ReportHomeDto
            {
                ProductNumber = numberProduct,
                BlogNumber = numberBlog,
                ContactNumber = numberContact
            };
            return result;
        }

        public async Task<ReportHomeDto> GetCountSideBar()
        {
            var numberContact = await _context.Contacts.CountAsync(x => x.Status == StatusEnum.InActive);
            var result = new ReportHomeDto
            {
                ContactNumber = numberContact
            };
            return result;
        }
    }
}