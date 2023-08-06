namespace BlogTask.Data.Queries
{
    public class UpdateArticleQuery
    {
        public string NewTitle { get; set; }
        public string NewText { get; set; }
        public UpdateArticleQuery(string newTitle, string newText)
        {
            NewTitle = newTitle;
            NewText = newText;
        }
    }
}
