using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Accounts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace Blog.Controllers
{
    public class AccountController : ControllerBase
    {
        private readonly TokenService _tokenService;  
        private readonly EmailService _emailService;
        public AccountController(TokenService tokenService, EmailService emailService)
        {
            _tokenService = tokenService;
            _emailService = emailService;
        }

        [HttpPost("v1/accounts/")]
        public async Task<IActionResult> PostAsync([FromBody] RegisterViewModel user, [FromServices] BlogDataContext dataContext)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var pass = PasswordGenerator.Generate(25);
            var userCreated = new User
            {
                Name = user.Name,
                Email = user.Email,
                Slug = user.Email.Replace("@", "-").Replace(".", "-"),
                PasswordHash = PasswordHasher.Hash(pass)
            };

            try
            {
                await dataContext.Users.AddAsync(userCreated);
                await dataContext.SaveChangesAsync();

                _emailService.Send(user.Name, user.Email, "Bem vindo ao blog", $"Sua senha é <strong>{pass}</strong>");

                return Created($"v1/accounts/{userCreated.Id}", new ResultViewModel<dynamic>(new
                {
                    user = userCreated.Email,
                    userCreated.Name,
                    userCreated.PasswordHash
                }));
            }
            catch (DbUpdateException)
            {
                return StatusCode(400, new ResultViewModel<string>("401A - Este usuario já está cadastrado"));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<string>("402A - Falha interna no servidor"));
            }

        }

        [HttpPost("v1/login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel login, [FromServices] BlogDataContext dbContext)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = await dbContext.
                Users
                .AsNoTracking()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Email == login.Email);

            if (user == null)
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha invalidas"));

            if(!PasswordHasher.Verify(user.PasswordHash, login.Password))
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha invalidas"));
            try
            {
                var token = _tokenService.GenerateToken(user);
                return Ok(new ResultViewModel<string>(token, null));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<string>("403A - Falha interna no servidor"));
            }
        }
    }
}
