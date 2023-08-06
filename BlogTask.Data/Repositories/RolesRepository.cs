using BlogTask.Data.Models;

namespace BlogTask.Data.Repositories
{
    public class RolesRepository : Repository<Role>
    {
        public RolesRepository(BlogContext db) : base(db)
        {
            
        }
    }
}
