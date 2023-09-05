using System.ComponentModel.DataAnnotations;

namespace BlogTask.Contracts.Models.Article
{
    public class EditArticleRequest
    {
        [Required]
        public Guid Guid { get; set; }
        [DataType(DataType.Text)]
        public string NewTitle { get; set; }
        [DataType(DataType.Text)]
        public string NewText { get; set; }
    }
}
