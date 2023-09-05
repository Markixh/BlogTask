using System.ComponentModel.DataAnnotations;

namespace BlogTask.Contracts.Models.Users
{
    public class EditUserRequest
    {
        [Required]
        public Guid Guid { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string NewLogin { get; set; }
        [DataType(DataType.Text)]
        public string NewFirstName { get; set; }
        [DataType(DataType.Text)]
        public string NewLastName { get; set; }
        [DataType(DataType.Text)]
        public string NewSurname { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
