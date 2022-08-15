using Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
        {
            return Ok(await context.Categories.ToListAsync());
        }
        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context,[FromRoute] int id)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)  
                return NotFound();

            return Ok(category);
        }
    }
}
