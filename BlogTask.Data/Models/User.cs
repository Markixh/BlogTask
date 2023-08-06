namespace BlogTask.Data.Models
{
    public class User
    {
        public Guid Guid { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Password { get; set; }
        public int? RoleId { get; set; }
        public Role? Role { get; set; }
    }
}
