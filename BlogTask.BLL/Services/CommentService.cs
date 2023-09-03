using BlogTask.Data.Models;

namespace BlogTask.BLL.Services
{
    public class CommentService : IService<Comment>
    {
        public Task CreateAsync(Comment item)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Comment item)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Comment>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Comment> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Comment> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Comment item)
        {
            throw new NotImplementedException();
        }
    }
}
