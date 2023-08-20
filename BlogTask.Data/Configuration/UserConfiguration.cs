using BlogTask.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogTask.Data.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        /// <summary>
        /// Конфигурация для таблицы UserFriends
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users").HasKey(p => p.Guid);

            builder.HasData(
            new User[]
            {
                new User { Guid = Guid.NewGuid(), FirstName = "Андрей", LastName = "Марков", Login = "Markov", Password = "pass", RoleId = 1},
                new User { Guid = Guid.NewGuid(), FirstName = "Елена", LastName = "Маркова", Login = "Markova", Password = "pass", RoleId = 2},
                new User { Guid = new Guid("E659CF76-3FAD-4795-AC68-7C73CA25835E"), FirstName = "Ольга", LastName = "Еремеева", Login = "Eremeeva", Password = "pass", RoleId = 3}
            });
        }
    }
}
