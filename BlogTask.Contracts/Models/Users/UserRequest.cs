using System.ComponentModel.DataAnnotations;

namespace BlogTask.Contracts.Models.Users
{
    public class UserRequest
    {
        [Required]
        public Guid Guid { get; set; }
        [Required]
        public string Login { get; set; }
        [DataType(DataType.Text)]
        public string FirstName { get; set; }
        [DataType(DataType.Text)]
        public string LastName { get; set; }
        [DataType(DataType.Text)]
        public string SurName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
