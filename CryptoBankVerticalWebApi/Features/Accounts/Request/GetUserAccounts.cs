using CryptoBankVerticalWebApi.Database;
using CryptoBankVerticalWebApi.Features.Accounts.Model;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CryptoBankVerticalWebApi.Features.Accounts.Request
{
    public static class GetUserAccounts
    {
        public record Request(long userId) : IRequest<Response>;

        public record Response(List<AccountModel> accountModels);

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator(ApplicationDbContext applicationDbContext)
            {
                ClassLevelCascadeMode = CascadeMode.Stop;
                RuleFor(x => x.userId).NotEmpty();
                RuleFor(x => x.userId).MustAsync(async (x, token) =>
                {
                    var isExistUser = await applicationDbContext.Users.AnyAsync(user => user.Id == x);
                    return isExistUser;
                }).WithMessage("Entered user non in system");
            }
        }

        public class RequestHandler : IRequestHandler<Request, Response>
        {
            private readonly ApplicationDbContext _applicationDbContext;

            public RequestHandler(ApplicationDbContext applicationDbContext)
            {
                _applicationDbContext = applicationDbContext;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var accounts = await _applicationDbContext.Accounts.Where(x => x.UserId == request.userId).ToListAsync();
                if (accounts.IsNullOrEmpty())
                {
                    return new Response(null);
                }
                List<AccountModel> accountModels = accounts.Select(account => new AccountModel()
                {
                    Id = account.Id,
                    UserId = account.UserId,
                    Amount = account.Amount,
                    Currency = account.Currency,
                    DateOfOpening = account.DateOfOpening
                }).ToList();
                return new Response(accountModels);
            }
        }
    }
}
