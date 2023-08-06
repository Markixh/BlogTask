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
        }
    }
}
