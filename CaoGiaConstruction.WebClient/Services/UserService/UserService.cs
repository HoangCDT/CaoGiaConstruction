using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services
{
    public interface IUserService
    {
        Task<Pager<User>> GetPaginationAsync(SearchKeywordPagination model);

        Task<OperationResult> ChangeStatus(Guid id);

        Task<OperationResult> RemoveAsync(Guid id);

        Task<User> FindByIdAsync(Guid id);

        Task<OperationResult> AddOrUpdateActionAsync(UserActionVM model);
    }

    public class UserService : IUserService, ITransientService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IFileService _fileService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private OperationResult _operationResult;

        public UserService(AppDbContext context,
            UserManager<User> userManager, RoleManager<Role> roleManager,
            IHttpContextAccessor contextAccessor,
            IMapper mapper, IFileService fileService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<Pager<User>> GetPaginationAsync(SearchKeywordPagination model)
        {
            var query = _userManager.Users.AsQueryable().AsNoTracking()
                .OrderByDescending(x => x.CreatedDate).AsQueryable();
            if (!model.Keyword.IsNullOrEmpty())
            {
                model.Keyword = model.Keyword.ToLower();
                query = query.Where(x => x.UserName.ToLower() == model.Keyword
                || x.FullName.ToLower() == model.Keyword
                || x.PhoneNumber.ToLower() == model.Keyword);
            }
            return await query.ToPaginationAsync(model);
        }

        public virtual async Task<OperationResult> ChangeStatus(Guid id)
        {
            var model = await _userManager.FindByIdAsync(id.ToString());
            if (model == null)
            {
                return new OperationResult(StatusCodes.Status400BadRequest, MessageReponse.NOT_FOUND_DATA);
            }

            model.Status = (model.Status == AccountStatus.Active ? AccountStatus.DeActive : AccountStatus.Active);
            try
            {
                var result = await _userManager.UpdateAsync(model);
                if (result.Succeeded)
                {
                    return new OperationResult(StatusCodes.Status200OK, MessageReponse.ADD_OR_UPDATE_SUCCESS);
                }
                else
                {
                    return new OperationResult(StatusCodes.Status400BadRequest, String.Join(" - ", result.Errors));
                }
            }
            catch (Exception ex)
            {
                return ex.GetMessageError();
            }
        }

        public virtual async Task<OperationResult> RemoveAsync(Guid id)
        {
            var data = await _userManager.FindByIdAsync(id.ToString());
            if (data != null)
            {
                var result = await _userManager.DeleteAsync(data);
                if (result.Succeeded)
                {
                    return new OperationResult(StatusCodes.Status200OK, MessageReponse.ADD_OR_UPDATE_SUCCESS);
                }
                else
                {
                    return new OperationResult(StatusCodes.Status400BadRequest, String.Join(" - ", result.Errors));
                }
            }
            return new OperationResult(StatusCodes.Status400BadRequest, MessageReponse.NOT_FOUND_DATA);
        }

        public virtual async Task<User> FindByIdAsync(Guid id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        public async Task<OperationResult> AddOrUpdateActionAsync(UserActionVM model)
        {
            var data = _mapper.Map<User>(model);

            bool isUploadFile = (model.File != null && model.File.Length > 0);
            if (isUploadFile)
            {
                var fileResult = await _fileService.UploadImageWithExtensionWebpAsync(model.File, $"{Commons.FILE_UPLOAD}/user/");
                if (fileResult != null && fileResult.Success)
                {
                    data.Avatar = fileResult.Data.ToString();
                }
            }

            if (model.Id != Guid.Empty)
            {
                var exist = await _userManager.FindByIdAsync(model.Id.ToString());
                if (exist != null)
                {
                    exist.FullName = data.FullName;
                    exist.Email = data.Email;
                    exist.PhoneNumber = data.PhoneNumber;
                    exist.Address = model.Address;
                    exist.Status = model.Status;
                    if (data.Avatar != exist.Avatar)
                    {
                        await _fileService.DeleteFileAsync(exist.Avatar);
                        exist.Avatar = data.Avatar;
                    }

                    var result = await _userManager.UpdateAsync(exist);
                    if (result.Succeeded)
                    {
                        if (!model.NewPassword.IsNullOrEmpty())
                        {
                            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(exist);
                            await _userManager.ResetPasswordAsync(exist, resetToken, model.NewPassword);
                        }
                        return new OperationResult(StatusCodes.Status200OK, MessageReponse.ADD_OR_UPDATE_SUCCESS);
                    }
                    else
                    {
                        return new OperationResult(StatusCodes.Status400BadRequest, String.Join(" - ", result.Errors));
                    }
                }
                else
                {
                    return new OperationResult(StatusCodes.Status400BadRequest, "Không tìm thấy tài khoản");
                }
            }
            else
            {
                var result = await _userManager.CreateAsync(data, model.Password);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    await _userManager.AddToRoleAsync(user, "Admin"); //Role mặc định là admin
                    return new OperationResult(StatusCodes.Status200OK, MessageReponse.ADD_OR_UPDATE_SUCCESS);
                }
                else
                {
                    return new OperationResult(StatusCodes.Status400BadRequest, String.Join(" - ", result.Errors));
                }
            }
        }
    }
}