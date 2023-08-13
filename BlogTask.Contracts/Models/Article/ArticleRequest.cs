namespace BlogTask.Contracts.Models.Article
{
    public class ArticleRequest
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public Guid UserGuid { get; set; }
    }
}
