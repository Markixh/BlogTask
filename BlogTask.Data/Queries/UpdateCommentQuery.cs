namespace BlogTask.Data.Queries
{
    public class UpdateCommentQuery
    {
        public string NewText { get; set; }
        public UpdateCommentQuery(string newText)
        {
            NewText = newText;
        }
    }
}
