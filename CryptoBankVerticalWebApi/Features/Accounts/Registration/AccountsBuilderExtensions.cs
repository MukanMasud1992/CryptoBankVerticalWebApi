using CryptoBankVerticalWebApi.Features.Accounts.Options;
using CryptoBankVerticalWebApi.Features.Users.Options;

namespace CryptoBankVerticalWebApi.Features.Accounts.Registration
{
    public static class AccountsBuilderExtensions
    {
        public static WebApplicationBuilder AddAccounts(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<AccountsOptions>(builder.Configuration.GetSection("Features:Accounts:AccountOptions"));
            return builder;
        }
    }
}
