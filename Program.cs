using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static Blog.Configuration;

var builder = WebApplication.CreateBuilder(args);

var key  = Encoding.ASCII.GetBytes(Configuration.JwtKey);
//configurar autenticação
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddDbContext<BlogDataContext>();
//injeção de dependencia
builder.Services.AddTransient<TokenService>();

var app = builder.Build();

Configuration.JwtKey = app.Configuration.GetValue<string>("JwtKey");
Configuration.ApiKeyName = app.Configuration.GetValue<string>("ApiKeyName");
Configuration.ApiKey = app.Configuration.GetValue<string>("ApiKey");

var smtp = new SmtpConfiguration();
app.Configuration.GetSection("smtp").Bind(smtp);
Configuration.smtpConfiguration = smtp;

//configura o uso de autenticação e autorização, sempre na order autenticação => autorização
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
