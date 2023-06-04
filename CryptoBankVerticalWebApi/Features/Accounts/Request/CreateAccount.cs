using CryptoBankVerticalWebApi.Database;
using CryptoBankVerticalWebApi.Features.Users.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using CryptoBankVerticalWebApi.Features.Accounts.Model;
using CryptoBankVerticalWebApi.Features.Accounts.Options;
using CryptoBankVerticalWebApi.Features.Accounts.Domain;

namespace CryptoBankVerticalWebApi.Features.Accounts.Request
{
    public static class CreateAccount
    {
        public record Request(CreateAccountModel createAccountModel) : IRequest<Response>;

        public record Response(AccountModel accountModel);

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator(ApplicationDbContext applicationDbContext)
            {
                ClassLevelCascadeMode = CascadeMode.Stop;
                RuleFor(x => x.createAccountModel.Amount).GreaterThan(0);
                RuleFor(x => x.createAccountModel.Currency).NotEmpty();
                RuleFor(x => x.createAccountModel.UserId).MustAsync(async (x, token) =>
                {
                    var isExistUser = await applicationDbContext.Users.AnyAsync(user => user.Id == x);
                    return isExistUser;
                }).WithMessage("Entered user non in system");
            }
        }

        public class RequestHandler : IRequestHandler<Request, Response>
        {
            private readonly ApplicationDbContext _applicationDbContext;
            private readonly IOptions<AccountsOptions> _options;

            public RequestHandler(ApplicationDbContext applicationDbContext, IOptions<AccountsOptions> options)
            {
                _applicationDbContext = applicationDbContext;
                _options = options;
            }
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var accounts = _applicationDbContext.Accounts.ToList().Count;
                if (accounts < _options.Value.MaxAccountsPerUser)
                {
                    var account = new Account()
                    {
                        Id = Guid.NewGuid(),
                        Amount = request.createAccountModel.Amount,
                        Currency = request.createAccountModel.Currency,
                        DateOfOpening = DateTime.UtcNow.ToUniversalTime(),
                        UserId = request.createAccountModel.UserId
                    };
                    _applicationDbContext.Accounts.Add(account);
                    _applicationDbContext.SaveChanges();
                    return new Response(new AccountModel()
                    {
                        Id = account.Id,
                        Amount = account.Amount,
                        Currency = account.Currency,
                        DateOfOpening = account.DateOfOpening,
                        UserId = account.UserId
                    });
                }
                else
                {
                    return new Response(null);
                }
            }
        }
    }
}
