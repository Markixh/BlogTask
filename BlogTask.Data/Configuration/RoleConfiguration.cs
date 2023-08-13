using BlogTask.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogTask.Data.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        /// <summary>
        /// Конфигурация для таблицы Roles
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles").HasKey(p => p.Id);
            builder.Property(x => x.Id).UseIdentityColumn();

            builder.HasData(
            new Role[]
            {
                new Role { Id = 1, Name = "Администратор", Description = "Роль дает возможность присваивать роли пользователям, так же создавать новые роли"},
                new Role { Id = 2, Name = "Модератор", Description = "Роль дает возможность вносить изменения во все статьи и коментарии" },
                new Role { Id = 3, Name = "Пользователь", Description = "Обычный пользователь без привилегий" }
            });
        }
    }
}
