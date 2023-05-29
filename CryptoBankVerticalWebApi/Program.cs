using CryptoBankVerticalWebApi.Database;
using CryptoBankVerticalWebApi.Features.Users.Registration;
using CryptoBankVerticalWebApi.Pipeline;
using CryptoBankVerticalWebApi.Pipeline.Behaviors;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationDbContext")));
builder.AddUsers();

builder.Services.AddMediatR(cfg => cfg
    .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
    // Can be merged if necessary
    .AddOpenBehavior(typeof(ValidationBehavior<,>)));

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddSingleton<Dispatcher>();
builder.Services.AddControllers();



var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
