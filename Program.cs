using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static Blog.Configuration;

var builder = WebApplication.CreateBuilder(args);

ConfigureAuthentication(builder);
ConfigureMvc(builder);
ConfigureServices(builder);

var app = builder.Build();

LoadConfiguration(app);

//configura o uso de autentica��o e autoriza��o, sempre na order autentica��o => autoriza��o
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
void ConfigureAuthentication(WebApplicationBuilder builder) {
    var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
    //configurar autentica��o
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
          });
}
void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddDbContext<BlogDataContext>();
    //inje��o de dependencia
    builder.Services.AddTransient<TokenService>();
}
