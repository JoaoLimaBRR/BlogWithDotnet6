using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAllAsync([FromServices] BlogDataContext context)
        {
            try
            {
                return Ok(await context.Categories.ToListAsync());
            }
            catch (Exception)
            {

                return StatusCode(500, "500C1 - Falha interna no servidor");
            }
            
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromServices] BlogDataContext context,[FromRoute] int id)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (category == null)
                    return NotFound();

                return Ok(category);
            }
            catch (Exception)
            {

                return StatusCode(500, "500C2 - Falha interna no servidor");
            }
           
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync([FromServices] BlogDataContext context, [FromBody] Category model)
        {

            try
            {
                await context.Categories.AddAsync(model);
                await context.SaveChangesAsync();

                return Created($"v1/categories/{model.Id}", model);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "500C3 - Não foi possivel incluir a categoria");
            }
            catch (Exception)
            {
                return StatusCode(500, "500C4 - Falha interna no servidor");
            }
           
        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync([FromServices] BlogDataContext context, [FromRoute] int id, [FromBody] Category model)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                    return BadRequest();

                category.Slug = model.Slug;
                category.Name = model.Name;

                context.Categories.Update(category);
                await context.SaveChangesAsync();

                return Ok(category);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "500C5 -Não foi possivel incluir a categoria");
            }
            catch (Exception)
            {
                return StatusCode(500, "500C6 - Falha interna no servidor");
            }

        }
        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromServices] BlogDataContext context, [FromRoute] int id)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                    return BadRequest();

                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(category);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "500C7 - Não foi possivel incluir a categoria");
            }
            catch (Exception)
            {
                return StatusCode(500, "500C8 - Falha interna no servidor");
            }

        }
    }
}
