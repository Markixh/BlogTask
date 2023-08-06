namespace BlogTask.Data.Models
{
    public class Tag
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public List<Article> Articles { get; set; }
    }
}
