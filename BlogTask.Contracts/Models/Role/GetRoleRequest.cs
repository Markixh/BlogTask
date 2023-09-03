namespace BlogTask.Contracts.Models.Role
{
    public class GetRoleResponse
    {
        public int RoleAmount { get; set; }
        public RoleView[] RoleView { get; set; }
    }
    public class RoleView
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
    }
}
