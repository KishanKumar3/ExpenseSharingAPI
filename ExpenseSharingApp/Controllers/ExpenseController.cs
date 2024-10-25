using ExpenseSharingApp.Interfaces;
using ExpenseSharingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSharingApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;


        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost]
        public IActionResult AddExpense([FromBody] ExpenseDetailsModel expenseDetails)
        {
            try
            {
                _expenseService.AddExpense(expenseDetails.GroupId,expenseDetails);
                return Ok("Expense added successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Expense>> GetAllExpenses()
        {
            var expenses = _expenseService.GetAllExpenses();
            return Ok(expenses);
        }

        [HttpGet("{expenseId}")]
        public ActionResult<Expense> GetExpenseById(string expenseId)
        {
            try
            {
                var expense = _expenseService.GetExpenseById(expenseId);
                return Ok(expense);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("group/{groupId}/expenses")]
        public IActionResult GetAllExpensesOfGroup(string groupId)
        {
            try
            {
                var expenses = _expenseService.GetAllExpensesOfGroup(groupId);
                return Ok(expenses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("{expenseId}")]
        public IActionResult UpdateExpense(string expenseId, [FromBody] ExpenseDetailsModel updatedExpenseDetails)
        {
            try
            {
                _expenseService.UpdateExpense(expenseId, updatedExpenseDetails);
                return Ok("Expense updated successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{expenseId}")]
        public IActionResult DeleteExpense(string expenseId)
        {
            try
            {
                _expenseService.DeleteExpense(expenseId);
                return Ok("Expense deleted successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("settle/{expenseId}")]
        public IActionResult SettleExpense(string expenseId)
        {
            try
            {
                _expenseService.SettleExpense(expenseId);
                return Ok("Expenses settled successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }









    }
}
