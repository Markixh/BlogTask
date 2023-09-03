using BlogTask.Data.Models;
using BlogTask.Data.Queries;
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

        public async Task<User> UpdateAsync(User user, UpdateUserQuery query)
        {
            if (!string.IsNullOrEmpty(query.NewLogin))
                user.Login = query.NewLogin;
            if (!string.IsNullOrEmpty(query.NewFirstName))
                user.FirstName = query.NewFirstName;
            if (!string.IsNullOrEmpty(query.NewLastName))
                user.LastName = query.NewLastName;
            if (!string.IsNullOrEmpty(query.NewSurName))
                user.SurName = query.NewSurName;
            if (!string.IsNullOrEmpty(query.NewPassword))
                user.Password = query.NewPassword;

            await UpdateAsync(user);
            return user;
        }
    }
}
