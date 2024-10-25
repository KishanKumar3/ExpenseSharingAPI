namespace ExpenseSharingApp.DTOs
{
    public class ExpenseDTO
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string PaidByUserId { get; set; }
    }
}
