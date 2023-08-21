using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace BlogTask.Data.Repositories
{
    public class ArticlesRepository: Repository<Article>
    {
        public ArticlesRepository(BlogContext db) : base(db)
        {

        }

        public Article UpdateByArticle(Article article, UpdateArticleQuery updateArticleQuery)
        {
            if (!string.IsNullOrEmpty(updateArticleQuery.NewTitle))
                article.Title = updateArticleQuery.NewTitle;
            if (!string.IsNullOrEmpty(updateArticleQuery.NewText))
                article.Text = updateArticleQuery.NewText;

            UpdateAsync(article).Wait();
            return article;
        }

        public Article GetWithTags(Guid guid) 
        {
            var article = Set.Where(a => a.Guid == guid).FirstOrDefault();

            _db.Entry(article).Collection(_ => _.Tags).Load();

            return article;
        }
    }
}
