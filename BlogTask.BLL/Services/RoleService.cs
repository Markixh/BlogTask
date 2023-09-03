using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using System.Data;

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

        public async Task DeleteAsync(Role role)
        {
            await _rolesRepository.DeleteAsync(role);
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

        public async Task<Role> UpdateAsync(Role role, UpdateRoleQuery query)
        {
            if (!string.IsNullOrEmpty(query.NewName))
                role.Name = query.NewName;
            if (!string.IsNullOrEmpty(query.NewDescription))
                role.Description = query.NewDescription;

            await UpdateAsync(role);
            return role;
        }
    }
}
