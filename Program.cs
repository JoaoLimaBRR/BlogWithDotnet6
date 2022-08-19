using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using static Blog.Configuration;

var builder = WebApplication.CreateBuilder(args);
ConfigureAuthentication(builder);
ConfigureMvc(builder);
ConfigureServices(builder);


var app = builder.Build();
//configura o uso de autenticação e autorização, sempre na order autenticação => autorização

LoadConfiguration(app);
////arquivos estaticos ---> wwwrot
//app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

void LoadConfiguration(WebApplication app)
{
    Configuration.JwtKey = app.Configuration.GetValue<string>("JwtKey");
    Configuration.ApiKeyName = app.Configuration.GetValue<string>("ApiKeyName");
    Configuration.ApiKey = app.Configuration.GetValue<string>("ApiKey");

    var smtp = new SmtpConfiguration();
    app.Configuration.GetSection("smtp").Bind(smtp);
    Configuration.smtpConfiguration = smtp;
}
void ConfigureAuthentication(WebApplicationBuilder builder)
{
    var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
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
}
void ConfigureMvc(WebApplicationBuilder builder)
{
    builder.
      Services
      .AddControllers()
      .ConfigureApiBehaviorOptions(
          options =>
          {
              options.SuppressModelStateInvalidFilter = true;
          })
      .AddJsonOptions( x =>
      {
          //ignora segundo ciclos de objetos, por exemplo
          //post tem author && author tem **post
          //dentro do objeto author, ele nao busca os posts dele, se não daria referencia circular infinita
          x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
          x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
      });
}
void ConfigureServices(WebApplicationBuilder builder)
{//injeção de dependencia
    builder.Services.AddDbContext<BlogDataContext>();
    builder.Services.AddTransient<TokenService>();
    builder.Services.AddTransient<EmailService>();
}
