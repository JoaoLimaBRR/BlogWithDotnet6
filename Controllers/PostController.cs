using Blog.Data;
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
    }
}
