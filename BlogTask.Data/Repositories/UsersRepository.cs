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
            var user = new User()
            {
                Guid = Guid.NewGuid(),
                FirstName = "Андрей",
                LastName = "Марков",
                Login = "Andrej",
                Password = "password",
                Role = new Role()
                {
                    Name = "Администратор"
                }
            };
            
            CreateAsync(user).Wait();
            

            user = new User()
            {
                Guid = Guid.NewGuid(),
                FirstName = "Елена",
                LastName = "Маркова",
                Login = "Elena",
                Password = "password",
                Role = new Role()
                {
                    Name = "Модератор"
                }
            };

            CreateAsync(user).Wait();

            user = new User()
            {
                Guid = Guid.NewGuid(),
                FirstName = "Пользователь",
                LastName = "Пользователь",
                Login = "user",
                Password = "password",
                Role = new Role()
                {
                    Name = "Пользователь"
                }
            };

            CreateAsync(user).Wait();
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
                user.Surname = updateUserQuery.NewSurName;
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
