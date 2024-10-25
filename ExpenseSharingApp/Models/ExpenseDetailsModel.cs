using System.ComponentModel.DataAnnotations;

namespace ExpenseSharingApp.Models
{
    public class ExpenseDetailsModel
    {

        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public string PaidById { get; set; }


        public string GroupId { get; set; }
    }
}
