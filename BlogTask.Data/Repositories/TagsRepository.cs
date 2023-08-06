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

        public Tag UpdateByTag(Tag tag, UpdateTagQuery updateTagQuery)
        {
            if (!string.IsNullOrEmpty(updateTagQuery.NewName))
                tag.Name = updateTagQuery.NewName;

            UpdateAsync(tag).Wait();

            return tag;
        }
    }
}
