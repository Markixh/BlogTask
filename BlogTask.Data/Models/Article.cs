namespace BlogTask.Data.Models
{
    public class Article
    {
        public Guid Guid { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public Guid UserGuid { get; set; }

        public User User { get; set; }

        public List<Tag> Tags { get; set; }
    }
}
