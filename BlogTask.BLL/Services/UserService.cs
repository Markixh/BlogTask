using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using System.Data;

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

        public async Task DeleteAsync(User user)
        {
            await _usersRepository.DeleteAsync(user);
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

        public async Task<User>? GetByLogin(string login)
        {
            return _usersRepository?.GetByLogin(login);
        }

        public async Task UpdateAsync(User user)
        {
            await _usersRepository.UpdateAsync(user);
        }
    }
}
