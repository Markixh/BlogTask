using System.ComponentModel.DataAnnotations;

namespace BlogTask.Contracts.Models.Tags
{
    public class GetTagResponse
    {
        public int TagAmount { get; set; }
        public TagView[] TagView { get; set; }
    }
    public class TagView
    {
        [Required]
        public Guid Guid { get; set; }
        [DataType(DataType.Text)]
        public string Name { get; set; }
    }
}
