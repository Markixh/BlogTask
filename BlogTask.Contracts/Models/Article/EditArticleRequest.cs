namespace BlogTask.Contracts.Models.Article
{
    public class EditArticleRequest
    {
        public Guid Guid { get; set; }
        public string NewTitle { get; set; }
        public string NewText { get; set; }
    }
}
