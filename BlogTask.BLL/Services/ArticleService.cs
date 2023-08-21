using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using System.Collections;

namespace BlogTask.BLL.Services
{
    public class ArticleService : IService<Article>
    {
        private readonly ArticlesRepository _articleRepository;
        private readonly UsersRepository _userRepository;
        private readonly TagsRepository _tagRepository;

        public ArticleService(IUnitOfWork unitOfWork) 
        {
            _articleRepository = unitOfWork.GetRepository<Article>() as ArticlesRepository;
            _userRepository = unitOfWork.GetRepository<User>() as UsersRepository;
            _tagRepository = unitOfWork.GetRepository<Tag>() as TagsRepository;
        }

        public Task CreateAsync(Article item)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Article item)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            var articles = _articleRepository.GetAll().ToArray();

            for (int i = 0; i < articles.Length; i++)
                articles[i] = _articleRepository.GetWithTags(articles[i].Guid);

            return articles;
        }

        public Task<Article> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Article> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Article item)
        {
            throw new NotImplementedException();
        }
    }
}
