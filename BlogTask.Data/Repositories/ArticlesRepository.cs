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

        public Article GetWithTags(Guid guid) 
        {
            var article = Set.Where(a => a.Guid == guid).FirstOrDefault();

            _db.Entry(article).Collection(_ => _.Tags).Load();

            return article;
        }
    }
}
