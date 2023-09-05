using System.ComponentModel.DataAnnotations;

namespace BlogTask.Contracts.Models.Role
{
    public class EditRoleRequest
    {
        [Required]
        public int Id { get; set; }
        [DataType(DataType.Text)]
        public string NewName { get; set; }
        [DataType(DataType.Text)]
        public string NewDescription { get; set; }
    }
}
