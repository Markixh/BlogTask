using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;

namespace BlogTask.BLL.Services
{
    public class UserService : IService<User>
    {
        private readonly UsersRepository _usersRepository;

        public UserService(IUnitOfWork unitOfWork)
        {
            _usersRepository = unitOfWork.GetRepository<User>() as UsersRepository;
        }

        public async Task CreateAsync(User user)
        {
            await _usersRepository.CreateAsync(user);
        }

        public Task DeleteAsync(User item)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return _usersRepository.GetAll().ToArray();
        }

        public Task<User> GetAsync(Guid id)
        {
            return _usersRepository.GetAsync(id);
        }

        public Task<User> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(User user)
        {
            await _usersRepository.UpdateAsync(user);
        }
    }
}
