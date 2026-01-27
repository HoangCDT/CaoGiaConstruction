using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.WebClient.Context.Entities;

namespace CaoGiaConstruction.WebClient.Context
{
    public class AppDbContext : IdentityDbContext<User, Role, Guid>
    {
        public readonly IHttpContextAccessor _contextAccessor;

        public AppDbContext(DbContextOptions options, IHttpContextAccessor contextAccessor = null) : base(options)
        {
            _contextAccessor = contextAccessor;
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<About> Abouts { get; set; }
        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<BlogCategory> BlogCategories { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Slide> Slides { get; set; }
        public virtual DbSet<SlideCategory> SlideCategories { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Branches> Branches { get; set; }
        public virtual DbSet<TimeLine> TimeLines { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<Properties> Properties { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductMainCategory> ProductMainCategories { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProductCategoryProperties> ProductCategoryProperties { get; set; }
        public virtual DbSet<ProductProperties> ProductProperties { get; set; }
        public virtual DbSet<Weight> Weight { get; set; }
        public virtual DbSet<ProductType> ProductType { get; set; }
        public virtual DbSet<ProductPrice> ProductPrice { get; set; }
        public virtual DbSet<DefineSystem> DefineSystem { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServiceCategory> ServiceCategories { get; set; }
        public virtual DbSet<Video> Videos { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<HomeComponentConfig> HomeComponentConfigs { get; set; }
        public virtual DbSet<MenuConfig> MenuConfigs { get; set; }
        public virtual DbSet<ProcessStep> ProcessSteps { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims").HasKey(x => x.Id);

            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims")
                .HasKey(x => x.Id);

            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins").HasKey(x => x.UserId);

            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles")
                .HasKey(x => new { x.RoleId, x.UserId });

            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens")
               .HasKey(x => new { x.UserId });

            builder.Entity<ProductProperties>()
           .Navigation(p => p.Properties)
           .AutoInclude();

            builder.Entity<ProductCategoryProperties>()
           .Navigation(p => p.Properties)
           .AutoInclude();

            builder.Entity<Product>()
           .Navigation(p => p.ProductProperties)
           .AutoInclude();

            builder.Entity<Product>()
           .Navigation(p => p.ProductPrices)
           .AutoInclude();

            builder.Entity<ProductProperties>()
          .HasOne(pp => pp.Product)
          .WithMany(p => p.ProductProperties)
          .HasForeignKey(pp => pp.ProductId)
          .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<HomeComponentConfig>()
                .ToTable("HomeComponentConfigs");

        }

        public override int SaveChanges()
        {
            //Tự động cập nhật ngày giờ thêm mới và chỉnh sửa
            AutoAddDateTracking();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            //Tự động cập nhật ngày giờ thêm mới và chỉnh sửa
            AutoAddDateTracking();
            return (await base.SaveChangesAsync(true, cancellationToken));
        }

        public void AutoAddDateTracking()
        {
            var modified = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);
            foreach (EntityEntry item in modified)
            {
                var changedOrAddedItem = item.Entity;
                if (changedOrAddedItem != null)
                {
                    if (item.State == EntityState.Added)
                    {
                        SetValueProperty(ref changedOrAddedItem, "CreatedDate", "CreatedBy");
                    }
                    if (item.State == EntityState.Modified)
                    {
                        SetValueProperty(ref changedOrAddedItem, "ModifiedDate", "ModifiedBy");
                    }
                }
            }
        }

        public void SetValueProperty(ref object changedOrAddedItem, string propDate, string propUser)
        {
            Type type = changedOrAddedItem.GetType();
            PropertyInfo propAdd = type.GetProperty(propDate);
            if (propAdd != null)
            {
                propAdd.SetValue(changedOrAddedItem, DateTime.UtcNow, null);
            }
            var httpContext = _contextAccessor.HttpContext;
            if (httpContext != null)
            {
                var userClaim = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "id");
                PropertyInfo propCreateBy = type.GetProperty(propUser);
                if (propCreateBy != null && userClaim != null)
                {
                    propCreateBy.SetValue(changedOrAddedItem, userClaim.Value.ToGuid(), null);
                }
            }
        }

    }
}