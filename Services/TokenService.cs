using Blog.Extensions;
using Blog.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Blog.Services
{
    public class TokenService
    {
        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            //captura todas as permissoes do usuario
            var claims = user.GetClaims();
            //transforma para bytes a key criada na classe de configuration
            var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
            //configura como o token ira se comportar
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //passa para o gerador de tokens as permissoes do usuario
                Subject = new ClaimsIdentity(claims),
                //expira em 8 horas
                Expires = DateTime.UtcNow.AddHours(8),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    //tipo de algoritimo pra encriptar o token
                    SecurityAlgorithms.HmacSha256Signature)
            };
            //cria o token em cima das configurações definidas a cima 
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //retorna ele em forma de string 
            return tokenHandler.WriteToken(token);
        }
    }
}
