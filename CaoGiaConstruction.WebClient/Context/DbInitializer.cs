using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Linq;
using CaoGiaConstruction.WebClient.Const;
using CaoGiaConstruction.WebClient.Context.Entities;
using CaoGiaConstruction.WebClient.Context.Enums;
using static CaoGiaConstruction.WebClient.Const.ProductMainCategoryCodeDefine;

namespace CaoGiaConstruction.WebClient.Context
{
    public class DbInitializer
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(AppDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager, ILogger<DbInitializer> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
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
                    var adminRole = await _roleManager.FindByNameAsync("Admin");
                    if (adminRole == null)
                    {
                        adminRole = new Role()
                        {
                            Name = "Admin",
                            NormalizedName = "ADMIN",
                            Description = "Top manager"
                        };
                        await _roleManager.CreateAsync(adminRole);
                    }
                    
                    // Kiểm tra trực tiếp trong database xem user đã có role chưa
                    var userRoleExists = await _context.Set<IdentityUserRole<Guid>>()
                        .AnyAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id);
                    
                    if (!userRoleExists)
                    {
                        // Thêm try-catch để tránh duplicate key exception (phòng trường hợp race condition)
                        try
                        {
                            await _userManager.AddToRoleAsync(adminUser, "Admin");
                            // Commit ngay để catch exception tại đây
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateException dbEx)
                        {
                            // Kiểm tra duplicate key exception (PostgreSQL error code 23505)
                            var innerException = dbEx.InnerException;
                            var isDuplicateKey = innerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505";
                            
                            if (isDuplicateKey)
                            {
                                _logger?.LogWarning("User admin already has Admin role (duplicate key), skipping AddToRoleAsync");
                            }
                            else
                            {
                                // Re-throw các lỗi khác
                                throw;
                            }
                        }
                        catch (Exception ex)
                        {
                            // Kiểm tra duplicate key exception (PostgreSQL error code 23505)
                            var isDuplicateKey = ex is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505";
                            
                            if (isDuplicateKey)
                            {
                                _logger?.LogWarning("User admin already has Admin role (duplicate key), skipping AddToRoleAsync");
                            }
                            else
                            {
                                // Re-throw các lỗi khác
                                throw;
                            }
                        }
                    }
                }
            }
            else
            {
                // Đảm bảo user admin có role Admin
                // Kiểm tra trực tiếp trong database để tránh race condition
                var adminRole = await _roleManager.FindByNameAsync("Admin");
                if (adminRole == null)
                {
                    adminRole = new Role()
                    {
                        Name = "Admin",
                        NormalizedName = "ADMIN",
                        Description = "Top manager"
                    };
                    await _roleManager.CreateAsync(adminRole);
                }
                
                // Kiểm tra trực tiếp trong database xem user đã có role chưa
                var userRoleExists = await _context.Set<IdentityUserRole<Guid>>()
                    .AnyAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id);
                
                if (!userRoleExists)
                {
                    // Thêm try-catch để tránh duplicate key exception (phòng trường hợp race condition)
                    try
                    {
                        await _userManager.AddToRoleAsync(adminUser, "Admin");
                        // Commit ngay để catch exception tại đây
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException dbEx)
                    {
                        // Kiểm tra duplicate key exception (PostgreSQL error code 23505)
                        var innerException = dbEx.InnerException;
                        var isDuplicateKey = innerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505";
                        
                        if (isDuplicateKey)
                        {
                            _logger?.LogWarning("User admin already has Admin role (duplicate key), skipping AddToRoleAsync");
                        }
                        else
                        {
                            // Re-throw các lỗi khác
                            throw;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Kiểm tra duplicate key exception (PostgreSQL error code 23505)
                        var isDuplicateKey = ex is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505";
                        
                        if (isDuplicateKey)
                        {
                            _logger?.LogWarning("User admin already has Admin role (duplicate key), skipping AddToRoleAsync");
                        }
                        else
                        {
                            // Re-throw các lỗi khác
                            throw;
                        }
                    }
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

                // HOME_SLIDE_TOP - Slider chính ở đầu trang chủ
                slideCategories.Add(new SlideCategory
                {
                    Code = SlideCategoryCodeDefine.HOME_SLIDE_TOP,
                    Title = "Slider Trang Chủ",
                    Description = "Slider chính hiển thị ở đầu trang chủ",
                    SortOrder = 1,
                    Status = StatusEnum.Active
                });

                // HOME_SLIDE_ACHIEVEMENT - Slider thành tựu
                slideCategories.Add(new SlideCategory
                {
                    Code = SlideCategoryCodeDefine.HOME_SLIDE_ACHIEVEMENT,
                    Title = "Slider Thành Tựu",
                    Description = "Slider hiển thị các thành tựu của công ty",
                    SortOrder = 2,
                    Status = StatusEnum.Active
                });

                // HOME_SLIDE_GALLERY - Slider thư viện ảnh
                slideCategories.Add(new SlideCategory
                {
                    Code = SlideCategoryCodeDefine.HOME_SLIDE_GALLERY,
                    Title = "Slider Thư Viện Ảnh",
                    Description = "Slider hiển thị thư viện ảnh",
                    SortOrder = 3,
                    Status = StatusEnum.Active
                });

                // HOME_SLIDE_PARTNER - Slider đối tác
                slideCategories.Add(new SlideCategory
                {
                    Code = SlideCategoryCodeDefine.HOME_SLIDE_PARTNER,
                    Title = "Slider Đối Tác",
                    Description = "Slider hiển thị các đối tác của công ty",
                    SortOrder = 4,
                    Status = StatusEnum.Active
                });

                // HOME_SLIDE_BRANCHES - Slider chi nhánh
                slideCategories.Add(new SlideCategory
                {
                    Code = SlideCategoryCodeDefine.HOME_SLIDE_BRANCHES,
                    Title = "Slider Chi Nhánh",
                    Description = "Slider hiển thị các chi nhánh",
                    SortOrder = 5,
                    Status = StatusEnum.Active
                });

                // HOME_BANNER_ABOUT - Banner giới thiệu
                slideCategories.Add(new SlideCategory
                {
                    Code = SlideCategoryCodeDefine.HOME_BANNER_ABOUT,
                    Title = "Banner Giới Thiệu",
                    Description = "Banner hiển thị phần giới thiệu",
                    SortOrder = 6,
                    Status = StatusEnum.Active
                });

                // HOME_BANNER_BLOG - Banner blog
                slideCategories.Add(new SlideCategory
                {
                    Code = SlideCategoryCodeDefine.HOME_BANNER_BLOG,
                    Title = "Banner Blog",
                    Description = "Banner hiển thị phần blog",
                    SortOrder = 7,
                    Status = StatusEnum.Active
                });

                await _context.SlideCategories.AddRangeAsync(slideCategories);
            }

            // Seed ProductMainCategory
            if (!_context.ProductMainCategories.Any())
            {
                var productMainCategories = new List<ProductMainCategory>
                {
                    new ProductMainCategory
                    {
                        Code = ProductMainCategoryCodeDefine.PRODUCT,
                        Title = "Sản Phẩm - Nguyên Liệu",
                        Description = "Danh mục sản phẩm và nguyên liệu",
                        SortOrder = 1,
                        Status = StatusEnum.Active
                    },
                    new ProductMainCategory
                    {
                        Code = ProductMainCategoryCodeDefine.MACHINES,
                        Title = "Máy Móc - Thiết Bị - Dụng Cụ",
                        Description = "Danh mục máy móc, thiết bị và dụng cụ",
                        SortOrder = 2,
                        Status = StatusEnum.Active
                    },
                    new ProductMainCategory
                    {
                        Code = ProductMainCategoryCodeDefine.GRINDER,
                        Title = "Máy Xay",
                        Description = "Danh mục máy xay",
                        SortOrder = 3,
                        Status = StatusEnum.Active
                    }
                };

                await _context.ProductMainCategories.AddRangeAsync(productMainCategories);
            }

            // Seed BlogCategory - Mẫu cho mỗi loại blog
            if (!_context.BlogCategories.Any())
            {
                var blogCategories = new List<BlogCategory>
                {
                    // Kiến thức
                    new BlogCategory
                    {
                        Code = "knowledge-general",
                        Title = "Kiến Thức Chung",
                        Description = "Các bài viết kiến thức chung về cà phê",
                        Type = BlogTypeEnum.KNOWLEDGE,
                        SortOrder = 1,
                        Status = StatusEnum.Active
                    },
                    // Tin tức - Sự kiện
                    new BlogCategory
                    {
                        Code = "news-general",
                        Title = "Tin Tức Chung",
                        Description = "Các tin tức và sự kiện",
                        Type = BlogTypeEnum.NEWSEVENT,
                        SortOrder = 1,
                        Status = StatusEnum.Active
                    },
                    // Công thức
                    new BlogCategory
                    {
                        Code = "formula-general",
                        Title = "Công Thức Chung",
                        Description = "Các công thức pha chế",
                        Type = BlogTypeEnum.FORMULA,
                        SortOrder = 1,
                        Status = StatusEnum.Active
                    }
                };

                await _context.BlogCategories.AddRangeAsync(blogCategories);
            }

            // Seed ServiceCategory - Mẫu
            if (!_context.ServiceCategories.Any())
            {
                var serviceCategories = new List<ServiceCategory>
                {
                    new ServiceCategory
                    {
                        Code = "service-general",
                        Title = "Dịch Vụ Chung",
                        Description = "Các dịch vụ của công ty",
                        SortOrder = 1,
                        Status = StatusEnum.Active
                    }
                };

                await _context.ServiceCategories.AddRangeAsync(serviceCategories);
            }

            // Seed Setting - Cài đặt mặc định
            if (!_context.Settings.Any())
            {
                var setting = new Setting
                {
                    PrimaryColor = "#007bff",
                    SecondaryColor = "#6c757d",
                    FooterBackgroundColor = "#2C3E50",
                    HeaderMenuTextColor = "#333333",
                    HeaderMenuTextSelectedColor = "#007bff",
                    HeaderMenuHoverColor = "#007bff",
                    FooterSubTextColor = "#CCCCCC",
                    SubMenuTextColor = "#333333",
                    SubMenuBorderTopColor = "#cdcdcd",
                    FontFamily = "Arial, sans-serif",
                    FontSize = "14px",
                    Status = StatusEnum.Active
                };

                await _context.Settings.AddAsync(setting);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Kiểm tra duplicate key exception (PostgreSQL error code 23505)
                var innerException = dbEx.InnerException;
                var isDuplicateKey = innerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505";
                
                if (isDuplicateKey)
                {
                    _logger?.LogWarning("Duplicate key detected during seeding (likely UserRoles), skipping. This is normal if data already exists.");
                    // Bỏ qua duplicate key exception - đây là trường hợp bình thường khi seed lại
                }
                else
                {
                    _logger?.LogError(dbEx, "Error occurred while saving changes during database seeding");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error occurred while saving changes during database seeding");
                throw;
            }
        }
    }
}