using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.Utilities.Dtos;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Extensions;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IContactService : IBaseService<Contact>
    {
        Task<Pager<Contact>> GetPaginationAsync(SearchKeywordPagination model);
    }

    public class ContactService : BaseService<Contact>, IContactService, ITransientService
    {
        private readonly AppDbContext _context;
        private IFileService _fileService;
        private readonly MapperConfiguration _configMapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;
        private OperationResult _operationResult;
        private readonly int _userId;

        public ContactService(AppDbContext context, MapperConfiguration configMapper, IHttpContextAccessor contextAccessor, IMapper mapper, IFileService fileService) : base(context)
        {
            _context = context;
            _configMapper = configMapper;
            _contextAccessor = contextAccessor;
            _userId = _contextAccessor.HttpContext.User.GetUserId();
            _mapper = mapper;
            _operationResult = new OperationResult();
            _fileService = fileService;
        }
        public override async Task<OperationResult> AddAsync(Contact model)
        {
            try
            {
                await base.AddAsync(model);
                return new OperationResult()
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Cảm ơn bạn đã liên hệ! Chúng tôi đã nhận được thông tin và sẽ phản hồi bạn trong thời gian sớm nhất.",
                };
            }
            catch (Exception ex)
            {
                return ex.GetMessageError();
            }
        }

        public async Task<Pager<Contact>> GetPaginationAsync(SearchKeywordPagination model)
        {
            var query = _context.Contacts.AsNoTracking()
                 .Include(x => x.UserCreated)
                  .OrderByDescending(x => x.CreatedDate).AsQueryable();
            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                query = query.Where(x => x.Title.ToLower().Contains(model.Keyword));
            }
            return await query.ToPaginationAsync(model);
        }
    }
}