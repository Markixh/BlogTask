using System.ComponentModel.DataAnnotations;

namespace BlogTask.Contracts.Models.Tags
{
    public class EditTagRequest
    {
        [Required]
        public Guid Guid { get; set; }
        [DataType(DataType.Text)]
        public string NewName { get; set; }
    }
}
