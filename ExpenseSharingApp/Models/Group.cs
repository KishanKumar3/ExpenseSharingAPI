using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseSharingApp.Models
{
    public class Group
    {
        public string GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<UserGroup> Members { get; set; } = new List<UserGroup>();

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();

      
        public decimal TotalExpense { get; set; }
    }
}
