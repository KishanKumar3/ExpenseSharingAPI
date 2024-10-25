using System.ComponentModel.DataAnnotations;

namespace ExpenseSharingApp.Models
{
    public class Expense
    {

        //desc
        //amount
        //paid by
        //date
        //splitAmong
        //group
        //settle
        [Key]
        public string Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public bool IsSettled { get; set; }

        public string PaidById { get; set; }


        public string GroupId { get; set; }

        public User PaidBy { get; set; }
        public Group Group { get; set; }

    }
}
