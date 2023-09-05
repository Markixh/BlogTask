using System.ComponentModel.DataAnnotations;

namespace BlogTask.Contracts.Models.Comment
{
    public class EditCommentRequest
    {
        [Required]
        public Guid Guid { get; set; }
        [DataType(DataType.Text)]
        public string NewText { get; set; }
    }
}
