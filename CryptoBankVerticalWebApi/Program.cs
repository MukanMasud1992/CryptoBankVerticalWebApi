using Auth.Authorization.Requirements;
using CryptoBankVerticalWebApi.Authorization.Requirements;
using CryptoBankVerticalWebApi.Database;
using CryptoBankVerticalWebApi.Features.Auth;
using CryptoBankVerticalWebApi.Features.Users.Domain;
using CryptoBankVerticalWebApi.Features.Users.Registration;
using CryptoBankVerticalWebApi.Pipeline.Behaviors;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using CryptoBankVerticalWebApi.Features.Accounts.Registration;
using System.Text;
using CryptoBankVerticalWebApi.Features.Auth.Registration;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMediatR(cfg => cfg
    .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
    .AddOpenBehavior(typeof(ValidationBehavior<,>)));

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationDbContext")));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtOptions = builder.Configuration.GetSection("Features:Auth").Get<AuthOptions>()!.Jwt;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtOptions.SigningKey)),
        };
    });


builder.Services.AddSingleton<IAuthorizationHandler, RoleRequirementHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNames.UserRole, policy => policy.RequireClaim(ClaimTypes.Role, UserRole.UserRole.ToString()));
    options.AddPolicy(PolicyNames.AnalystRole, policy => policy.RequireClaim(ClaimTypes.Role, UserRole.AnalystRole.ToString()));
    options.AddPolicy(PolicyNames.AdministratorRole, policy => policy.RequireClaim(ClaimTypes.Role, UserRole.AdministratorRole.ToString()));
});


builder.Services.AddControllers();

builder.AddUsers();
builder.AddAccounts();
builder.AddAuth();


var app = builder.Build();


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
