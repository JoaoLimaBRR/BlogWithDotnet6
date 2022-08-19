using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    public class PostController : ControllerBase
    {
        [HttpGet("v1/posts")]
        public async Task<IActionResult> GetAllAsync([FromServices] BlogDataContext dbContext, [FromQuery] int page = 0, [FromQuery] int pageSize = 25)
        {

            try
            {
                var postList = await dbContext
                                .Posts
                                .AsNoTracking()
                                .Include(x => x.Category)
                                .Include(x => x.Author)
                                .Select(x => new ListPostViewModel
                                {
                                    Id = x.Id,
                                    Title = x.Title,
                                    Slug = x.Slug,
                                    LastUpdateDate = x.LastUpdateDate,
                                    Category = x.Category.Name,
                                    Author = x.Author.Name
                                })
                                //pula a quantidade de casas passadas por parametro
                                .Skip(page * pageSize)
                                //pega uma quantidade de registro passada por parametro
                                .Take(pageSize)
                                .ToListAsync();

                var listSize = await dbContext.Posts.CountAsync();

                return Ok(new ResultViewModel<dynamic>
                    (new
                    {
                        listSize,
                        page,
                        pageSize,
                        postList
                    }
                    ));
            }
            catch (Exception)
            {

                return StatusCode(500, new ResultViewModel<string>("500P1 - Falha interna no servidor"));
            }

        }

        [HttpGet("v1/posts/{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromServices] BlogDataContext dbContext, [FromRoute] int id)
        {
            try
            {
                var post = await dbContext
                                .Posts
                                .AsNoTracking()
                                .Include(x => x.Author)
                                    .ThenInclude(x => x.Roles)
                                .Include(x => x.Category)
                                .FirstOrDefaultAsync(x => x.Id == id);

                if (post == null)
                    return NotFound(new ResultViewModel<string>("Post não encontrado"));

                return Ok(new ResultViewModel<Post>(post));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<string>("500P2 - Falha interna no servidor"));
            }

        }

        [HttpGet("v1/posts/category/{category}")]
        public async Task<IActionResult> GetByCategory([FromServices] BlogDataContext dbContext, [FromRoute] string category,
                                                       [FromQuery] int pageSize = 25, [FromQuery] int page = 0)
        {
            try
            {
                var posts = await dbContext
                                .Posts
                                .AsNoTracking()
                                .Include(x => x.Category)
                                //pula a quantidade de casas passadas por parametro
                                .Skip(page * pageSize)
                                //pega uma quantidade de registro passada por parametro
                                .Take(pageSize)
                                .Where(x => x.Category.Name == category)
                                .ToListAsync();

                var listSize = await dbContext
                    .Posts
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .Where(x => x.Category.Name == category)
                    .CountAsync();

                return Ok(new ResultViewModel<dynamic>
                    (new
                    {
                        listSize,
                        page,
                        pageSize,
                        posts
                    }
                    ));
            }
            catch (Exception)
            {

                return StatusCode(500, new ResultViewModel<string>("500P1 - Falha interna no servidor"));
            }

        }
    }
}
