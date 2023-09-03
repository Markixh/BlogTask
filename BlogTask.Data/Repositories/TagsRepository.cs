using BlogTask.Data;
using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using BlogTask.Data.Repositories;

namespace BlogTask.Data.Repositories
{
    public class TagsRepository : Repository<Tag>
    {
        public TagsRepository(BlogContext db) : base(db)
        {
        }

        public Tag UpdateByTag(Tag tag)
        {
            UpdateAsync(tag).Wait();

            return tag;
        }
    }
}
