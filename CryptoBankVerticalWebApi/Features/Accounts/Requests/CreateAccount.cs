using CryptoBankVerticalWebApi.Database;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using CryptoBankVerticalWebApi.Features.Accounts.Model;
using CryptoBankVerticalWebApi.Features.Accounts.Options;
using CryptoBankVerticalWebApi.Features.Accounts.Domain;

namespace CryptoBankVerticalWebApi.Features.Accounts.Requests
{
    public static class CreateAccount
    {
        public record Request(CreateAccountModel CreateAccountModel,long userId) : IRequest<Response>;

        public record Response(AccountModel AccountModel);

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator(ApplicationDbContext applicationDbContext)
            {
                RuleFor(x => x.CreateAccountModel.Currency).Cascade(CascadeMode.Stop).NotEmpty();
                RuleFor(x => x.userId).Cascade(CascadeMode.Stop).MustAsync(async (x, token) =>
                {
                    var isUserExist = await applicationDbContext.Users.AnyAsync(user => user.Id == x);
                    return isUserExist;
                }).WithMessage("User does not exist");
                RuleFor(x => x.CreateAccountModel.Number).Cascade(CascadeMode.Stop).MustAsync(async (x, token) =>
                {
                    var isAccountExist = await applicationDbContext.Accounts.AnyAsync(account => account.Number==x);
                    return !isAccountExist;
                }).WithMessage("An account with this number exists");
            }
        }

        public class RequestHandler : IRequestHandler<Request, Response>
        {
            private readonly ApplicationDbContext _applicationDbContext;
            private readonly AccountsOptions _options;

            public RequestHandler(ApplicationDbContext applicationDbContext, IOptions<AccountsOptions> options)
            {
                _applicationDbContext = applicationDbContext;
                _options = options.Value;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var accounts = await _applicationDbContext.Accounts.Where(a=>a.UserId==request.userId).CountAsync(cancellationToken);
                if (accounts < _options.MaxAccountsPerUser)
                {
                    var account = new Account()
                    {
                        Number = request.CreateAccountModel.Number,
                        Amount = 0,
                        Currency = request.CreateAccountModel.Currency,
                        CreatedAt = DateTime.UtcNow.ToUniversalTime(),
                        UserId = request.userId
                    };
                    await _applicationDbContext.Accounts.AddAsync(account,cancellationToken);
                    await _applicationDbContext.SaveChangesAsync(cancellationToken);
                    return new Response(new AccountModel()
                    {
                        Id = account.Id,
                        Amount = account.Amount,
                        Currency = account.Currency,
                        CreatedAt = account.CreatedAt,
                        UserId = account.UserId
                    });
                }
                throw new Exception("The user has accounts more then five");
            }
        }
    }
}
