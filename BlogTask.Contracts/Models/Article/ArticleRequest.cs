using System.ComponentModel.DataAnnotations;

namespace BlogTask.Contracts.Models.Article
{
    public class ArticleRequest
    {
        [DataType(DataType.Text)]
        public string Title { get; set; }
        [DataType(DataType.Text)]
        public string Text { get; set; }
    }
}
