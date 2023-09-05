using System.ComponentModel.DataAnnotations;

namespace BlogTask.Contracts.Models.Article
{
    public class GetCommentResponse
    {
        public int CommentAmount { get; set; }
        public CommentView[]? CommentView { get; set; }
    }
    public class CommentView
    {
        [Required]
        public Guid Guid { get; set; }
        [DataType(DataType.Text)]
        public string Text { get; set; } = string.Empty;
        [Required]
        public Guid? UserGuid { get; set; }
        [Required]
        public Guid ArticleGuid { get; set; }
    }
}
