namespace BlogTask.Contracts.Models.Tags
{
    public class EditTagRequest
    {
        public Guid Guid { get; set; }
        public string NewName { get; set; }
    }
}
