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
            public Guid Guid { get; set; }
            public string Login { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Surname { get; set; }
        }
    }
}
