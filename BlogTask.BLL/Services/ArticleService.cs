using Azure.Core;
using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using System;

namespace BlogTask.BLL.Services
{
    public class ArticleService : IService<Article>
    {
        private readonly ArticlesRepository _articlesRepository;

        public ArticleService(IUnitOfWork unitOfWork) 
        {
            _articlesRepository = unitOfWork.GetRepository<Article>() as ArticlesRepository;
        }

        public async Task CreateAsync(Article article)
        {
            await _articlesRepository.CreateAsync(article);
        }

        public async Task DeleteAsync(Article article)
        {
            await _articlesRepository.DeleteAsync(article);
        }

        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            var articles = _articlesRepository.GetAll().ToArray();

            for (int i = 0; i < articles.Length; i++)
                articles[i] = _articlesRepository.GetWithTags(articles[i].Guid);

            return articles;
        }

        public async Task<Article> GetAsync(Guid id)
        {
            return _articlesRepository.GetWithTags(id);
        }

        public Task<Article> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Article> UpdateAsync(Article article, UpdateArticleQuery query)
        {
            if (!string.IsNullOrEmpty(query.NewTitle))
                article.Title = query.NewTitle;
            if (!string.IsNullOrEmpty(query.NewText))
                article.Text = query.NewText;

            await UpdateAsync(article);
            return article;
        }

        public async Task UpdateAsync(Article article)
        {
            await _articlesRepository.UpdateAsync(article);
        }
    }
}
