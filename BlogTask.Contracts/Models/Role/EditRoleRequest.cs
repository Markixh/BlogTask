namespace BlogTask.Contracts.Models.Role
{
    public class EditRoleRequest
    {
        public int Id { get; set; }
        public string NewName { get; set; }
        public string NewDescription { get; set; }
    }
}
