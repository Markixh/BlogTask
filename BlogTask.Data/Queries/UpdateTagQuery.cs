namespace BlogTask.Data.Queries
{
    public class UpdateTagQuery
    {
        public string NewName { get; set; }

        public UpdateTagQuery(string newName)
        {
            NewName = newName;
        }
    }
}
