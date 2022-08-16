using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class AccountController : ControllerBase
    {
        private readonly TokenService _tokenService;    
        public AccountController(TokenService tokenService)
        {
            _tokenService = tokenService;   
        }

        [HttpPost("v1/accounts/")]
        public async Task<IActionResult> PostAsync([FromBody] RegisterViewModel user, [FromServices] BlogDataContext dataContext)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var userCreated = new User
            {
                Name = user.Name,
                Email = user.Email,
                Slug = user.Email.Replace("@", "-").Replace(".", "-")
            };
            await dataContext.Users.AddAsync(userCreated);
            await dataContext.SaveChangesAsync();

            return Created($"v1/accounts/{userCreated.Id}", new ResultViewModel<User>(userCreated));
        }

        [HttpPost("v1/login")]
        public IActionResult Login()
        {
            var token = _tokenService.GenerateToken(null);

            return Ok(token);
        }
    }
}
