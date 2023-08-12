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
                new Role { Id = 1, Name = "Администратор"},
                new Role { Id = 2, Name = "Модератор" },
                new Role { Id = 3, Name = "Пользователь" }
            });
        }
    }
}
