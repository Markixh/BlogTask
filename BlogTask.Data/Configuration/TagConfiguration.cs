using BlogTask.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogTask.Data.Configuration
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        /// <summary>
        /// Конфигурация для таблицы UserFriends
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("Tags").HasKey(p => p.Guid);
        }
    }
}
