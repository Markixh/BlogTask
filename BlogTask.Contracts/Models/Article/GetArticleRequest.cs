using BlogTask.Contracts.Models.Tags;

namespace BlogTask.Contracts.Models.Article
{
    public class GetAeticleResponse
    {
        public int ArticleAmount { get; set; }
        public ArticleView[] ArticleView { get; set; }
    }
    public class ArticleView
    {
        public Guid Guid { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public Guid UserGuid { get; set; }
        public TagView[] Tags { get; set; }
    }
}
