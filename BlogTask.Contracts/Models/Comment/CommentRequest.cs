using System.ComponentModel.DataAnnotations;

namespace BlogTask.Contracts.Models.Comment
{
    public class CommentRequest
    {        
        [DataType(DataType.Text)]
        public string Text { get; set; }
        [Required]
        public Guid ArticleGuid { get; set; }
    }
}
