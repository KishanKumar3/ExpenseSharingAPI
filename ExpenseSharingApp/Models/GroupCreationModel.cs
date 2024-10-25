namespace ExpenseSharingApp.Models
{
    public class GroupCreationModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> MemberEmails { get; set; }
    }
}
