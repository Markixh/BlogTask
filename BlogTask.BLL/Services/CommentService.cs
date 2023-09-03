using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;

namespace BlogTask.BLL.Services
{
    public class CommentService : IService<Comment>
    {
        private readonly CommentsRepository _commentsRepository;

        public CommentService(IUnitOfWork unitOfWork)
        {
            _commentsRepository = unitOfWork.GetRepository<Comment>() as CommentsRepository;
        }

        public async Task CreateAsync(Comment comment)
        {
            await _commentsRepository.CreateAsync(comment);
        }

        public Task DeleteAsync(Comment item)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            var comments = _commentsRepository.GetAll().ToArray();
                        
            return comments;
        }

        public Task<Comment> GetAsync(Guid id)
        {
            return _commentsRepository.GetAsync(id);
        }

        public Task<Comment> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Comment> UpdateAsync(Comment comment, UpdateCommentQuery query)
        {
            if (!string.IsNullOrEmpty(query.NewText))
                comment.Text = query.NewText;

            await UpdateAsync(comment);
            return comment;
        }

        public async Task UpdateAsync(Comment comment)
        {
            await _commentsRepository.UpdateAsync(comment);
        }
    }
}
