namespace BlogTask.Contracts.Models.Article
{
    public class GetCommentResponse
    {
        public int CommentAmount { get; set; }
        public CommentView[]? CommentView { get; set; }
    }
    public class CommentView
    {
        public Guid Guid { get; set; }
        public string Text { get; set; } = string.Empty;
        public Guid? UserGuid { get; set; }
        public Guid ArticleGuid { get; set; }
    }
}
