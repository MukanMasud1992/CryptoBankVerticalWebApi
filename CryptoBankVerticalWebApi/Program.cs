using Auth.Authorization.Requirements;
using CryptoBankVerticalWebApi.Authorization.Requirements;
using CryptoBankVerticalWebApi.Database;
using CryptoBankVerticalWebApi.Features.Auth.Options;
using CryptoBankVerticalWebApi.Features.Users.Domain;
using CryptoBankVerticalWebApi.Features.Users.Registration;
using CryptoBankVerticalWebApi.Pipeline.Behaviors;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Claims;
using CryptoBankVerticalWebApi.Features.Accounts.Registration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMediatR(cfg => cfg
    .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
    // Can be merged if necessary
    .AddOpenBehavior(typeof(ValidationBehavior<,>)));

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationDbContext")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        builder.Configuration.GetSection(JWTSettings.JWTSectionName).Bind(JWTSettings.JWT);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(JWTSettings.JWT.SigningKey)),
            ValidateIssuer = true,
            ValidIssuer = JWTSettings.JWT.Issuer,
            ValidateAudience = true,
            ValidAudience = JWTSettings.JWT.Audience,
        };
    });
builder.Services.AddSingleton<IAuthorizationHandler, RoleRequirementHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNames.UserRole, policy => policy.AddRequirements(new RoleRequirement(UserRole.UserRole)));
    options.AddPolicy(PolicyNames.AnalystRole, policy => policy.AddRequirements(new RoleRequirement(UserRole.AnalystRole)));
    options.AddPolicy(PolicyNames.AdministratorRole, policy => policy.AddRequirements(new RoleRequirement(UserRole.AdministratorRole)));


    options.AddPolicy(PolicyNames.UserRole, policy => policy.RequireClaim(ClaimTypes.Role, UserRole.UserRole.ToString()));
    options.AddPolicy(PolicyNames.AnalystRole, policy => policy.RequireClaim(ClaimTypes.Role, UserRole.AnalystRole.ToString()));
    options.AddPolicy(PolicyNames.AdministratorRole, policy => policy.RequireClaim(ClaimTypes.Role, UserRole.AdministratorRole.ToString()));
});


//builder.Services.AddSingleton<Dispatcher>();
builder.Services.AddControllers();

builder.AddUsers();
builder.AddAccounts();


var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
