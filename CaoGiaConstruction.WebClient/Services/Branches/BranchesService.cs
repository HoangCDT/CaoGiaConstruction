using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.Utilities.Constants;
using CaoGiaConstruction.Utilities.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IBranchesService : IBaseService<Branches>
    {
        Task<OperationResult> AddOrUpdateActionAsync(BranchesActionVM model);
    }

    public class BranchesService : BaseService<Branches>, IBranchesService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;


        public BranchesService(AppDbContext context, IMapper mapper, IFileService fileService) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
        }

        public override async Task<OperationResult> RemoveAsync(Guid id)
        {
            var data = await FindByIdAsync(id);
            if (data != null && !data.Avatar.IsNullOrEmpty())
            {
                await _fileService.DeleteFileAsync(data.Avatar);
            }
            if (data != null)
            {
                return new OperationResult(StatusCodes.Status400BadRequest, "Chi nhánh không tồn tại");
            }

            return await base.RemoveAsync(id);
        }


        public async Task<OperationResult> AddOrUpdateActionAsync(BranchesActionVM model)
        {
            var data = _mapper.Map<Branches>(model);

            bool isUploadFile = model.File != null && model.File.Length > 0;
            if (isUploadFile)
            {
                var fileResult = await _fileService.UploadImageWithExtensionWebpAsync(model.File, $"{Commons.FILE_UPLOAD}/branches/");
                if (fileResult != null && fileResult.Success)
                {
                    data.Avatar = fileResult.Data.ToString();
                }
            }
            if (model.Id != Guid.Empty)
            {
                var exist = await _context.Branches.AsNoTracking()
                                        .Where(x => x.Id == model.Id)
                                        .FirstOrDefaultAsync();
                if (exist != null)
                {
                    data.CreatedBy = exist.CreatedBy;
                    data.ModifiedBy = exist.ModifiedBy;
                    data.CreatedDate = exist.CreatedDate;
                    data.ModifiedDate = exist.ModifiedDate;
                    if (data.Avatar != exist.Avatar)
                    {
                        await _fileService.DeleteFileAsync(exist.Avatar);
                    }

                    _context.Branches.Update(data);
                }
                else
                {
                    _context.Branches.Add(data);
                }
            }
            else
            {
                if (data.SortOrder == null)
                {
                    var maxPosition = await _context.Branches.MaxAsync(x => x.SortOrder);
                    data.SortOrder = (maxPosition ?? 0) + 1;
                }
                _context.Branches.Add(data);
            }
            try
            {
                await _context.SaveChangesAsync();
                return new OperationResult(StatusCodes.Status200OK, MessageReponse.ADD_OR_UPDATE_SUCCESS);
            }
            catch (Exception ex)
            {
                return ex.GetMessageError();
            }
        }
    }
}