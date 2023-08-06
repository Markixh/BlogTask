namespace BlogTask.Contracts.Models.Comment
{
    public class CommentRequest
    {
        public Guid Guid { get; set; }
        public string Text { get; set; }
        public Guid UserGuid { get; set; }
        public Guid ArticleGuid { get; set; }
    }
}
