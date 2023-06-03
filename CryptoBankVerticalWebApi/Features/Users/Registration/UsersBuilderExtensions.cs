using CryptoBankVerticalWebApi.Features.Users.Options;
using CryptoBankVerticalWebApi.Features.Users.Services;

namespace CryptoBankVerticalWebApi.Features.Users.Registration;

public static class UsersBuilderExtensions
{
    public static WebApplicationBuilder AddUsers(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<UsersOptions>(builder.Configuration.GetSection("Features:Users"));
        builder.Services.Configure<PasswordHashingOptions>(builder.Configuration.GetSection("Features:PasswordHashingOptions"));
        builder.Services.AddTransient<PasswordHeshingService>();
        return builder;
    }
}
