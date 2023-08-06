namespace BlogTask.Contracts.Models.Comment
{
    public class EditCommentRequest
    {
        public Guid Guid { get; set; }
        public string NewText { get; set; }
    }
}
