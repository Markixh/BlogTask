using BlogTask.Contracts.Models.Tags;
using System.ComponentModel.DataAnnotations;

namespace BlogTask.Contracts.Models.Article
{
    public class GetArticleResponse
    {
        public int ArticleAmount { get; set; }
        public ArticleView[] ArticleView { get; set; }
    }
    public class ArticleView
    {
        [Required]
        public Guid Guid { get; set; }
        [DataType(DataType.Text)]
        public string Title { get; set; }
        [DataType(DataType.Text)]
        public string Text { get; set; }
        [Required]
        public Guid UserGuid { get; set; }
        public TagView[] Tags { get; set; }
    }
}
