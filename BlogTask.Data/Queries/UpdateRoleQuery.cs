namespace BlogTask.Data.Queries
{
    public class UpdateRoleQuery
    {
        public string NewName { get; set; }
        public string NewDescription { get; set; }

        public UpdateRoleQuery(string newName, string newDescription)
        {
            NewName = newName;
            NewDescription = newDescription;
        }
    }
}
