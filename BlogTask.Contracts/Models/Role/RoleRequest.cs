using System.ComponentModel.DataAnnotations;

namespace BlogTask.Contracts.Models.Role
{
    public class RoleRequest
    {
        [DataType(DataType.Text)]
        public string Name { get; set; }
        [DataType(DataType.Text)]
        public string Description { get; set; }
        [Required]
        public int Id { get; set; }
    }
}
