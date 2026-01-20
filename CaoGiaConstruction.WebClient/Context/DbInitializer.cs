using Microsoft.AspNetCore.Identity;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Context.Enums;

namespace CaoGiaConstruction.WebClient.Context
{
    public class DbInitializer
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public DbInitializer(AppDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Seed()
        {
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new Role()
                {
                    Name = "Admin",
                    NormalizedName = "Admin",
                    Description = "Top manager"
                });
                await _roleManager.CreateAsync(new Role()
                {
                    Name = "Staff",
                    NormalizedName = "Staff",
                    Description = "Staff"
                });
                await _roleManager.CreateAsync(new Role()
                {
                    Name = "Customer",
                    NormalizedName = "Customer",
                    Description = "Customer"
                });
            }
            // Seed Admin User - Đảm bảo user admin luôn tồn tại
            var adminUser = await _userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                adminUser = new User()
                {
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    FullName = "Administrator",
                    Email = "admin@gmail.com",
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    EmailConfirmed = true,
                    CreatedDate = DateTime.UtcNow,
                    Status = AccountStatus.Active,
                    Address = "BD",
                    Avatar = "",
                    PhoneNumber = "",
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0
                };

                var result = await _userManager.CreateAsync(adminUser, "admin@123");
                if (result.Succeeded)
                {
                    // Đảm bảo role Admin tồn tại trước khi gán
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                    {
                        await _roleManager.CreateAsync(new Role()
                        {
                            Name = "Admin",
                            NormalizedName = "ADMIN",
                            Description = "Top manager"
                        });
                    }
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            else
            {
                // Đảm bảo user admin có role Admin
                var isInRole = await _userManager.IsInRoleAsync(adminUser, "Admin");
                if (!isInRole)
                {
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                    {
                        await _roleManager.CreateAsync(new Role()
                        {
                            Name = "Admin",
                            NormalizedName = "ADMIN",
                            Description = "Top manager"
                        });
                    }
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            if (!_context.Abouts.Any())
            {
                var about = new About
                {
                    LogoTop = "Client/images/logo.png",
                    LogoBottom = "Client/images/logo.png",
                    Status = StatusEnum.Active,
                    PhoneNumber = "0650558466",
                    PhoneNumberOther = "3591779",
                    Email = "support@abc.vn",
                    AboutUs = "Cao Gia Construction",
                    Content = "Cao Gia Construction",
                    Address = " 750 Lê Hồng Phong, phường Phú Thọ, tp Thủ Dầu Một, tỉnh Bình Dương",
                    Copyright = "©2024 Bản quyền thuộc về CaoGiaConstruction",
                };

                await _context.Abouts.AddAsync(about);
            }

            if (!_context.SlideCategories.Any())
            {
                var slideCategories = new List<SlideCategory>();

                var slideCategoryHomeTop = new SlideCategory
                {
                    Code = SlideCategoryCodeDefine.HOME_SLIDE_TOP,
                    Title = SlideCategoryCodeDefine.HOME_SLIDE_TOP,
                    SortOrder = 1,
                    Status = StatusEnum.Active
                };

                var slideCategoryHomeBlog = new SlideCategory
                {
                    Code = SlideCategoryCodeDefine.HOME_BANNER_BLOG,
                    Title = SlideCategoryCodeDefine.HOME_BANNER_BLOG,
                    SortOrder = 1,
                    Status = StatusEnum.Active
                };


                var slideCategoryGallery = new SlideCategory
                {
                    Code = SlideCategoryCodeDefine.HOME_SLIDE_GALLERY,
                    Title = SlideCategoryCodeDefine.HOME_SLIDE_GALLERY,
                    SortOrder = 2,
                    Status = StatusEnum.Active
                };

                var slideCategoryBranches = new SlideCategory
                {
                    Code = SlideCategoryCodeDefine.HOME_SLIDE_BRANCHES,
                    Title = SlideCategoryCodeDefine.HOME_SLIDE_BRANCHES,
                    SortOrder = 3,
                    Status = StatusEnum.Active
                };

                var homeSlideAbout = new SlideCategory
                {
                    Code = SlideCategoryCodeDefine.HOME_BANNER_ABOUT,
                    Title = SlideCategoryCodeDefine.HOME_BANNER_ABOUT,
                    SortOrder = 4,
                    Status = StatusEnum.Active
                };

                var homeSlideInfo = new SlideCategory
                {
                    Code = SlideCategoryCodeDefine.HOME_BANNER_BLOG,
                    Title = SlideCategoryCodeDefine.HOME_BANNER_BLOG,
                    SortOrder = 5,
                    Status = StatusEnum.Active
                };

                slideCategories.Add(homeSlideAbout);
                slideCategories.Add(homeSlideInfo);
                slideCategories.Add(slideCategoryGallery);

                await _context.SlideCategories.AddRangeAsync(slideCategories);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}