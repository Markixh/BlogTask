using AutoMapper;
using BlogTask.Contracts.Models.Article;
using BlogTask.Contracts.Models.Comment;
using BlogTask.Contracts.Models.Tags;
using BlogTask.Contracts.Models.Users;
using BlogTask.Data.Models;
using BlogTask.Models.Account;
using static BlogTask.Contracts.Models.Users.GetUserRequest;

namespace BlogTask
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRequest, User>();
            CreateMap<User, UserRequest>();
            CreateMap<ArticleRequest, Article>();
            CreateMap<Article, ArticleRequest>();
            CreateMap<CommentRequest, Comment>();
            CreateMap<Comment, CommentRequest>();
            CreateMap<TagRequest, Tag>();
            CreateMap<Tag, TagRequest>();

            CreateMap<User, UserView>();
            CreateMap<UserView, User>();
            CreateMap<Article, ArticleView>();
            CreateMap<Tag, TagView>();

            CreateMap<LoginViewModel, User>();
            CreateMap<RegisterViewModel, User>();

            CreateMap<Article, Models.Article.ArticleViewModel>();
        }
    }
}
