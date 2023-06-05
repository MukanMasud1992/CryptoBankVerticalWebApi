using CryptoBankVerticalWebApi.Features.Auth.Services;
using CryptoBankVerticalWebApi.Features.Users.Options;

namespace CryptoBankVerticalWebApi.Features.Auth.Registration
{
    public static class AuthBuilderExtensions
    {
        public static WebApplicationBuilder AddAuth(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Features:Auth"));
            builder.Services.AddTransient<TokenGenerateService>();
            return builder;
        }

    }
}
