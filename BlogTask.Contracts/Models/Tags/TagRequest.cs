using System.ComponentModel.DataAnnotations;

namespace BlogTask.Contracts.Models.Tags
{
    public class TagRequest
    {
        [Required]
        public Guid Guid { get; set; }
        [DataType(DataType.Text)]
        public string Name { get; set; }
    }
}
