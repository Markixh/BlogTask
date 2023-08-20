using BlogTask.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogTask.Data.Configuration
{
    public class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        /// <summary>
        /// Конфигурация для таблицы UserFriends
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.ToTable("Articles").HasKey(p => p.Guid);

            builder.HasData(
            new Article[]
            {
                new Article { 
                    Guid = Guid.NewGuid(), 
                    Title = "C# – когда программирование это просто, удобно и красиво.", 
                    Text = "Всем привет! Потихоньку мы опускаемся вниз по рейтингу TIOBE и переходим к языку, который замыкает пятерку самых популярных языков программирования – С#.\r\n\r\nЛоготип языка C#\r\nС# – это относительно молодой язык программирования, созданный компанией Microsoft в 2002 году. Как молодой язык многое унаследовал от уже имеющихся языков, покоривших ИТ-индустрию: Java, Smalltalk и Delphi. Популярность C# получил, прежде всего, из-за своей простоты. Да, это конечно не Python, но и до C ему очень далеко. С# обладает большим количеством разных «примочек» укорачивающих и упрощающих код. Одним из самых удачных примеров являются геттеры и сеттеры, которые в C# реализованы очень просто и понятно. Прежде всего, на C# разрабатываются игры, что обусловлено наличием платформы Unity, которая замечательно интегрируется с языком. Наиболее сильно удобство языка можно оценки разрабатывая Windows-приложения. В частности, практически все современные приложения под ОС Windows, включая Microsoft Office, Skype, Adobe Photoshop и многие другие написаны на C#. Этому способствует и наличие платформы .NET Framework, а позднее и .NET Core. Также, благодаря фреймворкам и технологиям WPF, Xamarin, ASP.NET, Entity язык C# прекрасно справляется с разработкой современных интерфейсов, мобильных приложений, серверной логики веб-сайтов, со взаимодействием с базами данных.", 
                    UserGuid = new Guid("E659CF76-3FAD-4795-AC68-7C73CA25835E"
                    )}
            });
        }
    }
}
