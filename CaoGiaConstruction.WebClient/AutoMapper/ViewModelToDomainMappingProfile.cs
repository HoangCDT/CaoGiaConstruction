using AutoMapper;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using CaoGiaConstruction.WebClient.Context.Entities;

namespace CaoGiaConstruction.WebClient.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<UserVM, User>();
            CreateMap<UserActionVM, User>();
            CreateMap<RoleVM, Role>();

            CreateMap<AboutVM, About>();
            CreateMap<AboutActionVM, About>();

            CreateMap<ContactVM, Contact>();
            CreateMap<FeedbackVM, Feedback>();
            CreateMap<FeedbackActionVM, Feedback>();

            CreateMap<ProductVM, Product>();
            CreateMap<ProductActionVM, Product>();
            CreateMap<ProductCategoryVM, ProductCategory>();
            CreateMap<ProductCategoryActionVM, ProductCategory>();
            CreateMap<ProductMainCategoryVM, ProductMainCategory>();

            CreateMap<ServiceVM, Service>();
            CreateMap<ServiceActionVM, Service>();
            CreateMap<ServiceCategoryVM, ServiceCategory>();
            CreateMap<ServiceCategoryActionVM, ServiceCategory>();
            CreateMap<ServiceNoContentVM, Service>();

            CreateMap<BlogVM, Blog>();
            CreateMap<BlogActionVM, Blog>();
            CreateMap<BlogCategoryVM, BlogCategory>();
            CreateMap<BlogCategoryActionVM, BlogCategory>();

            CreateMap<SlideVM, Slide>();
            CreateMap<SlideActionVM, Slide>();
            CreateMap<SlideCategoryVM, SlideCategory>();

            CreateMap<Properties, PropertiesVM>().ReverseMap();
            CreateMap<ProductCategoryPropertiesVM, ProductCategoryProperties>();
            CreateMap<ProductProperties, ProductPropertiesVM>().ReverseMap();

            CreateMap<Weight, WeightVM>();
            CreateMap<ProductType, ProductTypeVM>();
            CreateMap<ProductPrice, ProductPriceVM>();

            CreateMap<DefineSystem, DefineSystemVM>();
            CreateMap<SettingVM, Setting>();

            CreateMap<BranchesVM, Branches>().ReverseMap();
            CreateMap<BranchesActionVM, Branches>().ReverseMap();
            CreateMap<BranchesActionVM, BranchesVM>().ReverseMap();

            CreateMap<VideoVM, Video>().ReverseMap();
            CreateMap<VideoActionVM, Video>().ReverseMap();
            CreateMap<VideoActionVM, VideoVM>().ReverseMap();

            CreateMap<ProjectVM, Project>();
            CreateMap<ProjectActionVM, Project>();

        }
    }
}