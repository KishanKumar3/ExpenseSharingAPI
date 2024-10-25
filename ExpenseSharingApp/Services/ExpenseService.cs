using ExpenseSharingApp.Interfaces;
using ExpenseSharingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSharingApp.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ExpenseSharingContext _context;

        public ExpenseService(ExpenseSharingContext context)
        {
            _context = context;
        }



        public void AddExpense(string groupId, ExpenseDetailsModel expenseDetails)
        {
            // Fetch the group along with its UserGroups from the database
            var group = _context.Groups
                                .Include(g => g.Members)
                                .ThenInclude(ug => ug.User)
                                .FirstOrDefault(g => g.GroupId == expenseDetails.GroupId);

            var paidBy = _context.Users.FirstOrDefault(u => u.Id == expenseDetails.PaidById);

            if (group == null || paidBy == null)
            {
                throw new ArgumentException("Invalid expense details. Ensure the group and payer are valid.");
            }

            // Create a new expense
            var expense = new Expense
            {
                Id = Guid.NewGuid().ToString(),
                Description = expenseDetails.Description,
                Amount = expenseDetails.Amount,
                Date = expenseDetails.Date,
                PaidById = expenseDetails.PaidById,
                GroupId = expenseDetails.GroupId,
                IsSettled = false,
                PaidBy = paidBy,
                Group = group
            };

            // Use a transaction to ensure data consistency
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Expenses.Add(expense);
                    _context.SaveChanges();

                    // Calculate the amount each user should pay
                    var usersInGroup = group.Members.Where(ug => ug.UserId != expenseDetails.PaidById).ToList();
                    var totalUsers = usersInGroup.Count + 1; // Including the payer
                    decimal splitAmount = expense.Amount / totalUsers;

                    // Update amountToPay for each user in the group (except the payer)
                    foreach (var userGroup in usersInGroup)
                    {
                        userGroup.AmountToPay += splitAmount;
                    }

                    // Update balance for the payer
                    paidBy.Balance -= expenseDetails.Amount;

                    group.TotalExpense += expenseDetails.Amount;

                    // Save changes to the database
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Handle any exceptions and roll back the transaction if necessary
                    transaction.Rollback();
                    throw new ApplicationException("Failed to add expense. Transaction rolled back.", ex);
                }
            }
        }

        public IEnumerable<Expense> GetAllExpenses()
        {
            return _context.Expenses
                           .ToList();
        }

        public Expense GetExpenseById(string expenseId)
        {
            var expense = _context.Expenses
                                  .FirstOrDefault(e => e.Id == expenseId);

            if (expense == null)
            {
                throw new KeyNotFoundException("Expense not found.");
            }

            return expense;
        }

        public IEnumerable<Expense> GetAllExpensesOfGroup(string groupId)
        {
            // Fetch all expenses for the specified group
            var expenses = _context.Expenses
                                   .Include(e => e.PaidBy)
                                   .Where(e => e.GroupId == groupId)
                                   .ToList();

            return expenses;
        }



        public void UpdateExpense(string expenseId, ExpenseDetailsModel updatedExpenseDetails)
        {
            var expense = _context.Expenses.FirstOrDefault(e => e.Id == expenseId);

            if (expense == null)
            {
                throw new KeyNotFoundException("Expense not found.");
            }

            var group = _context.Groups
                                .Include(g => g.Members)
                                .ThenInclude(ug => ug.User)
                                .FirstOrDefault(g => g.GroupId == updatedExpenseDetails.GroupId);

            var paidBy = _context.Users.FirstOrDefault(u => u.Id == updatedExpenseDetails.PaidById);

            if (group == null || paidBy == null)
            {
                throw new ArgumentException("Invalid expense details. Ensure the group and payer are valid.");
            }

            // Update expense details
            expense.Description = updatedExpenseDetails.Description;
            expense.Amount = updatedExpenseDetails.Amount;
            expense.Date = updatedExpenseDetails.Date;
            expense.PaidById = updatedExpenseDetails.PaidById;
            expense.GroupId = updatedExpenseDetails.GroupId;
            expense.PaidBy = paidBy;
            expense.Group = group;

            _context.SaveChanges();
        }


        public void DeleteExpense(string expenseId)
        {
            var expense = _context.Expenses.FirstOrDefault(e => e.Id == expenseId);

            if (expense == null)
            {
                throw new KeyNotFoundException("Expense not found.");
            }

            _context.Expenses.Remove(expense);
            _context.SaveChanges();
        }

        public void SettleExpense(string expenseId)
        {

            // Fetch the expense from the database
            var expense = _context.Expenses
                                  .Include(e => e.Group)
                                  .Include(e => e.PaidBy)
                                  .FirstOrDefault(e => e.Id == expenseId);

            if (expense == null)
            {
                throw new Exception("Expense not found");
            }
            var group = _context.Groups
                        .Include(g => g.Members)
                        .ThenInclude(m => m.User)
                        .FirstOrDefault(g => g.GroupId == expense.GroupId);

            // Calculate split amount among group members (excluding the payer)
            var numberOfGroupMembers = group.Members.Count;
            var splitAmount = expense.Amount / numberOfGroupMembers;

            // Update balances for the user who paid the expense
            var payer = expense.PaidBy;
            payer.Balance += expense.Amount - splitAmount;


            // Update balances for all other group members
            foreach (var userGroup in expense.Group.Members)
            {
                var user = userGroup.User;
                if (user.Id != payer.Id)
                {
                    user.Balance -= splitAmount;
                    userGroup.AmountToPay = 0;
                }
            }

            // Mark the expense as settled
            expense.IsSettled = true;


            _context.SaveChanges();
        }
    }

}
