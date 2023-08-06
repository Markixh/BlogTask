using BlogTask.Data.Models;
using BlogTask.Data.Queries;

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
    }
}
