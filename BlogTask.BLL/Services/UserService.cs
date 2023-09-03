using BlogTask.Data.Models;

namespace BlogTask.BLL.Services
{
    public class UserService : IService<User>
    {
        public Task CreateAsync(User item)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(User item)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(User item)
        {
            throw new NotImplementedException();
        }
    }
}
