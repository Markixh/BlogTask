namespace BlogTask.Data.Models
{
    public class Tag
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Article>? Articles { get; set; }
    }
}
