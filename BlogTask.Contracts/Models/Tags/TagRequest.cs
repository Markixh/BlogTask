using System.ComponentModel.DataAnnotations;

namespace BlogTask.Contracts.Models.Tags
{
    public class TagRequest
    {
        [DataType(DataType.Text)]
        public string Name { get; set; }
    }
}
