using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;

namespace BlogTask.BLL.Services
{
    public class RoleService : IService<Role>
    {
        private readonly RolesRepository _rolesRepository;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _rolesRepository = unitOfWork.GetRepository<Role>() as RolesRepository;
        }

        public async Task CreateAsync(Role role)
        {
            await _rolesRepository.CreateAsync(role);
        }

        public Task DeleteAsync(Role item)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return _rolesRepository.GetAll().ToArray();
        }

        public Task<Role> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Role> GetAsync(int id)
        {
            return _rolesRepository.GetAsync(id);
        }

        public async Task UpdateAsync(Role role)
        {
            await _rolesRepository.UpdateAsync(role);
        }
    }
}
