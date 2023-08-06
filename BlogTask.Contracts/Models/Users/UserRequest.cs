namespace BlogTask.Contracts.Models.Users
{
    public class UserRequest
    {
        public Guid Guid { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SurName { get; set; }
        public string Password { get; set; }
    }
}
