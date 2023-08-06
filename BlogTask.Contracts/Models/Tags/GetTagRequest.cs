namespace BlogTask.Contracts.Models.Tags
{
    public class GetTagResponse
    {
        public int TagAmount { get; set; }
        public TagView[] TagView { get; set; }
    }
    public class TagView
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
    }
}
