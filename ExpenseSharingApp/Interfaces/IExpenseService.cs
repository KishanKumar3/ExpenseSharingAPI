using ExpenseSharingApp.Models;

namespace ExpenseSharingApp.Interfaces
{
    public interface IExpenseService
    {
        void AddExpense(string groupId, ExpenseDetailsModel expenseDetails);
        IEnumerable<Expense> GetAllExpenses();
        Expense GetExpenseById(string id);
        void UpdateExpense(string expenseId ,ExpenseDetailsModel expense);
        void DeleteExpense(string id);
        void SettleExpense(string expenseId);
        IEnumerable<Expense> GetAllExpensesOfGroup(string groupId);
    }

}
