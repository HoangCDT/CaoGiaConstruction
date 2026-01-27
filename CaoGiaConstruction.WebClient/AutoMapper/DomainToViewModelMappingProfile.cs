using AutoMapper;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Context.Entities;

namespace CaoGiaConstruction.WebClient.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<User, UserVM>();
            CreateMap<Role, RoleVM>();

            CreateMap<About, AboutVM>();
            CreateMap<Contact, ContactVM>();
            CreateMap<Feedback, FeedbackVM>();

            CreateMap<Product, ProductVM>();
            CreateMap<Product, ProductNoContentVM>();
            CreateMap<ProductCategory, ProductCategoryVM>();
            CreateMap<ProductMainCategory, ProductMainCategoryVM>();

            CreateMap<Blog, BlogVM>();
            CreateMap<Blog, BlogNoContentVM>();
            CreateMap<BlogCategory, BlogCategoryVM>();

            CreateMap<Slide, SlideVM>();
            CreateMap<SlideCategory, SlideCategoryVM>();

            CreateMap<Service, ServiceVM>();
            CreateMap<Service, ServiceActionVM>();

            CreateMap<Service, ServiceNoContentVM>();
            CreateMap<ServiceCategory, ServiceCategoryVM>();
            CreateMap<ServiceCategory, ServiceCategoryActionVM>();
            CreateMap<ProductCategoryProperties, ProductCategoryPropertiesVM>();

            CreateMap<Branches, BranchesVM>();

            CreateMap<Setting, SettingVM>();
            CreateMap<ProductCategoryProperties, ProductProperties>();

            CreateMap<Project, ProjectVM>();
            CreateMap<Project, ProjectNoContentVM>();

            CreateMap<ProcessStep, ProcessStepVM>();
            CreateMap<ProcessStep, ProcessStepActionVM>();
        }
    }
}