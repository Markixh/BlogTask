using BlogTask.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogTask.Data.Configuration
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        /// <summary>
        /// Конфигурация для таблицы Comments
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Comment> builder)
        {           
            builder.ToTable("Comments").HasKey(p => p.Guid);
        }
    }
}
