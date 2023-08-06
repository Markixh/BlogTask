namespace BlogTask.Data.Models
{
    public class Comment
    {
        public Guid Guid { get; set; }

        public string Text { get; set; } = string.Empty;

        public Guid? UserGuid { get; set; }

        public User? User { get; set; }

        public Guid ArticleGuid { get; set; }

        public Article Article { get; set; }
    }
}
