using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace BlogTask.Data.Repositories
{
    public class UsersRepository : Repository<User>
    {
        public UsersRepository(BlogContext db) : base(db)
        {
        }

        public User UpdateByUser(User user, UpdateUserQuery updateUserQuery)
        {
            if (!string.IsNullOrEmpty(updateUserQuery.NewLogin))
                user.Login = updateUserQuery.NewLogin;
            if (!string.IsNullOrEmpty(updateUserQuery.NewFirstName))
                user.FirstName = updateUserQuery.NewFirstName;
            if (!string.IsNullOrEmpty(updateUserQuery.NewLastName))
                user.LastName = updateUserQuery.NewLastName;
            if (!string.IsNullOrEmpty(updateUserQuery.NewSurName))
                user.SurName = updateUserQuery.NewSurName;
            if (!string.IsNullOrEmpty(updateUserQuery.NewPassword))
                user.Password = updateUserQuery.NewPassword;

            UpdateAsync(user).Wait();

            return user;
        }

        public User GetByLogin(string login) 
        {
            var user = this.GetAll().AsEnumerable().FirstOrDefault(x => x.Login == login);

            return user;
        }
    }
}
