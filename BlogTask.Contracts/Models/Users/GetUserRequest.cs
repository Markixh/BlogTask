using System.ComponentModel.DataAnnotations;

namespace BlogTask.Contracts.Models.Users
{
    public class GetUserRequest
    {
        public class GetUserResponse
        {
            public int UserAmount { get; set; }
            public UserView[] UserView { get; set; }
        }
        public class UserView
        {
            [Required]
            public Guid Guid { get; set; }
            [Required]
            [DataType(DataType.Text)]
            public string Login { get; set; }
            [DataType(DataType.Text)]
            public string FirstName { get; set; }
            [DataType(DataType.Text)]
            public string LastName { get; set; }
            [DataType(DataType.Text)]
            public string Surname { get; set; }
        }
    }
}
