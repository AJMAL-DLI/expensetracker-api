using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
   [Authorize] // This ensures only authenticated requests are allowed
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {

        public CategoriesController(Context context)
        {
            Context = context;
        }
        public Context Context { get; }


        [HttpGet("GetUserCategories")]
        public async Task<IActionResult> GetUserCategories()
        {
            // Extract the UserId from the JWT token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var categories = await Context.Categories
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return Ok(categories);
        }


        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await Context.Categories.OrderByDescending(t => t.Id).ToListAsync();

            return Ok(categories);
        }


        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody] Categories category)
        {
            // Extract the UserId from the JWT token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            // Assign the current user's ID to the category
            category.UserId = userId;

            Context.Categories.Add(category);
            await Context.SaveChangesAsync();

            return Ok(category);
        }

        [HttpPut("UpdateCategory/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Categories updatedCategory)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var existingCategory = await Context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (existingCategory == null)
            {
                return NotFound("Category not found or access denied.");
            }

            existingCategory.Name = updatedCategory.Name;

            await Context.SaveChangesAsync();

            return Ok(existingCategory);
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var category = await Context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (category == null)
            {
                return NotFound("Category not found or access denied.");
            }

            Context.Categories.Remove(category);
            await Context.SaveChangesAsync();

            return Ok(category);
        }



    }
}
