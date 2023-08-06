using BlogTask.Data.Models;
using BlogTask.Data.Queries;

namespace BlogTask.Data.Repositories
{
    public class CommentsRepository : Repository<Comment>
    {
        public CommentsRepository(BlogContext db) : base(db)
        {
        }

        public Comment UpdateByComment(Comment comment, UpdateCommentQuery updateCommentQuery)
        {
            if (!string.IsNullOrEmpty(updateCommentQuery.NewText))
                comment.Text = updateCommentQuery.NewText;

            UpdateAsync(comment).Wait();

            return comment;
        }
    }
}
