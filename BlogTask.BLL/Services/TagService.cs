using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;

namespace BlogTask.BLL.Services
{
    public class TagService : IService<Tag>
    {
        private readonly TagsRepository _tagsRepository;

        public TagService(IUnitOfWork unitOfWork)
        {
            _tagsRepository = unitOfWork.GetRepository<Tag>() as TagsRepository;
        }

        public async Task CreateAsync(Tag tag)
        {
            await _tagsRepository.CreateAsync(tag);
        }

        public Task DeleteAsync(Tag item)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return _tagsRepository.GetAll().ToArray();
        }

        public Task<Tag> GetAsync(Guid id)
        {
            return _tagsRepository.GetAsync(id);
        }

        public Task<Tag> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Tag tag)
        {
            await _tagsRepository.UpdateAsync(tag);
        }
    }
}
