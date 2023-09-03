using AutoMapper;
using BlogTask.Data.Models;
using BlogTask.Models.Account;

namespace BlogTask
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LoginViewModel, User>();
            CreateMap<RegisterViewModel, User>();
            CreateMap<Models.Article.AddViewModel, Article>();
            CreateMap<Models.Role.AddViewModel, Role>();
            CreateMap<Models.Tag.AddViewModel, Tag>();

            CreateMap<Article, Models.Article.ArticleViewModel>();
            CreateMap<Article, Models.Article.EditViewModel>();
            CreateMap<Role, Models.Role.RoleViewModel>();
            CreateMap<Role, Models.Role.EditViewModel>();
            CreateMap<Tag, Models.Tag.TagViewModel>();
            CreateMap<Tag, Models.Tag.EditViewModel>();
            CreateMap<User, UserViewModel>();
            CreateMap<User, EditUserVeiwModel>();
        }
    }
}
