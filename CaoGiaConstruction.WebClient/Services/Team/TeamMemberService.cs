using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.Utilities.Constants;
using CaoGiaConstruction.Utilities.Dtos;
using CaoGiaConstruction.WebClient.Areas.Admin.Dtos;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Context;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Context.Enums;
using CaoGiaConstruction.WebClient.Extensions;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface ITeamMemberService : IBaseService<TeamMember>
    {
        Task<List<TeamMember>> GetAllMembersAsync();
        Task<TeamMember> GetFounderAsync();
        Task<OperationResult> AddOrUpdateActionAsync(TeamMemberActionVM model);
        Task<OperationResult> UpdateSortOrderAsync(List<TeamMemberSortDto> items);
    }

    public class TeamMemberService : BaseService<TeamMember>, ITeamMemberService, ITransientService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public TeamMemberService(AppDbContext context, IFileService fileService, IMapper mapper) : base(context)
        {
            _context = context;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<List<TeamMember>> GetAllMembersAsync()
        {
            return await _context.TeamMembers
                .AsNoTracking()
                .Where(x => !x.IsFounder)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
        }

        public async Task<TeamMember> GetFounderAsync()
        {
            return await _context.TeamMembers
                .AsNoTracking()
                .Where(x => x.IsFounder)
                .OrderBy(x => x.SortOrder) // Just in case there are multiple, usually one
                .FirstOrDefaultAsync();
        }

        public async Task<OperationResult> AddOrUpdateActionAsync(TeamMemberActionVM model)
        {
            var data = _mapper.Map<TeamMember>(model);

            bool isUploadFile = (model.File != null && model.File.Length > 0);
            if (isUploadFile)
            {
                var fileResult = await _fileService.UploadImageWithExtensionWebpAsync(model.File, $"{Commons.FILE_UPLOAD}/team/");
                if (fileResult != null && fileResult.Success)
                {
                    data.Avatar = fileResult.Data.ToString();
                }
            }

            if (model.Id != Guid.Empty)
            {
                var exist = await _context.TeamMembers.AsNoTracking().Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (exist != null)
                {
                    data.CreatedBy = exist.CreatedBy;
                    data.ModifiedBy = exist.ModifiedBy;
                    data.CreatedDate = exist.CreatedDate;
                    data.ModifiedDate = exist.ModifiedDate;
                    if (isUploadFile && data.Avatar != exist.Avatar)
                    {
                        await _fileService.DeleteFileAsync(exist.Avatar);
                    }
                    else if (!isUploadFile)
                    {
                        data.Avatar = exist.Avatar;
                    }

                    _context.TeamMembers.Update(data);
                }
                else
                {
                    _context.TeamMembers.Add(data);
                }
            }
            else
            {
                if (data.SortOrder == 0)
                {
                    var maxPosition = await _context.TeamMembers.MaxAsync(x => (int?)x.SortOrder);
                    data.SortOrder = (maxPosition ?? 0) + 1;
                }
                _context.TeamMembers.Add(data);
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

        public async Task<OperationResult> UpdateSortOrderAsync(List<TeamMemberSortDto> items)
        {
            if (items == null || items.Count == 0)
            {
                return new OperationResult(StatusCodes.Status400BadRequest, "Dữ liệu không hợp lệ");
            }

            var ids = items.Select(x => x.Id).ToList();
            var entities = await _context.TeamMembers.Where(x => ids.Contains(x.Id)).ToListAsync();

            foreach (var entity in entities)
            {
                var item = items.FirstOrDefault(x => x.Id == entity.Id);
                if (item != null)
                {
                    entity.SortOrder = item.SortOrder;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return new OperationResult(StatusCodes.Status200OK, MessageReponse.UPDATE_SUCCESS);
            }
            catch (Exception ex)
            {
                return ex.GetMessageError();
            }
        }
    }
}
