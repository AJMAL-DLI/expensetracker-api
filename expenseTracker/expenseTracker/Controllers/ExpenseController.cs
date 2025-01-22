using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace expenseTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {

        private readonly Context _context;

        public ExpenseController(Context context)
        {
            _context = context;
        }

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] Expense expense)
        {
            // Extract the user ID from the JWT token
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            // Assign the UserId and CreatedOn timestamp
            expense.UserId = userId;
            expense.CreatedOn = DateTime.UtcNow;

            // Add or update the transaction
            if (expense.Id == 0)
            {
                _context.Expense.Add(expense);
            }
            else
            {
                _context.Expense.Update(expense);
            }

            await _context.SaveChangesAsync();
            return Ok(expense);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            // Extract the user ID from the JWT token
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            // Get transactions for the authenticated user
            var expense = await _context.Expense
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            return Ok(expense);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Extract the user ID from the JWT token
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            // Find the transaction belonging to the authenticated user
            var transaction = await _context.Expense
                .Where(t => t.Id == id && t.UserId == userId)
                .FirstOrDefaultAsync();

            if (transaction == null)
            {
                return NotFound("Transaction not found or you are not authorized.");
            }

            _context.Expense.Remove(transaction);
            await _context.SaveChangesAsync();
            return Ok("Deleted successfully.");
        }

        [HttpGet("GetMonthlyTransactions")]
        public async Task<IActionResult> GetMonthlyTransactions()
        {
            // Extract the user ID from the JWT token
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            // Get transactions for the authenticated user
            var expense = await _context.Expense
                .Where(t => t.UserId == userId)
                .ToListAsync();

            // Group by year and month
            var monthlyExpense = expense
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Expense = g.ToList()
                })
                .OrderByDescending(t => t.Year)
                .ThenByDescending(t => t.Month)
                .ToList();

            return Ok(monthlyExpense);
        }
    }
}
