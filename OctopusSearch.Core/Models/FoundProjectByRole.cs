namespace OctopusSearch.Core.Models
{
    public class FoundProjectByRole
    {
        public string Project { get; set; }
        public string[] AllRoles { get; set; }
        public string[] FoundRoles { get; set; }
    }
}
