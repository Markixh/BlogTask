namespace BlogTask.Contracts.Models.Users
{
    public class EditUserRequest
    {
        public Guid Guid { get; set; }
        public string NewLogin { get; set; }
        public string NewFirstName { get; set; }
        public string NewLastName { get; set; }
        public string NewSurname { get; set; }
        public string NewPassword { get; set; }
    }
}
