using CryptoBankVerticalWebApi.Database;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CryptoBankVerticalWebApi.Features.Accounts.Requests
{
    public class GetAccountsByPeriod
    {
        public record Request(DateTime start,DateTime end):IRequest<Response>;
        public record Response(int count);
        public class RequestValidator:AbstractValidator<Request>
        {
            public RequestValidator(ApplicationDbContext applicationDbContext) 
            {
                RuleFor(x => x.start)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty();

                RuleFor(x=>x.end)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty();

                RuleFor(x => new { x.start,x.end})
                    .Cascade(CascadeMode.Stop)
                    .Must(pair => IsValidRange(pair.start, pair.end))
                    .WithMessage("Invalid range");
            }

            private bool IsValidRange(DateTime start, DateTime end)
            {
                return start<end;
            }
        }

        public class RequestHandler : IRequestHandler<Request, Response>
        {
            private readonly ApplicationDbContext _applicationDbContext;

            public RequestHandler(ApplicationDbContext applicationDbContext)
            {
                _applicationDbContext=applicationDbContext;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var count = await _applicationDbContext.Accounts
                    .Where(account => account.CreatedAt>=request.start && account.CreatedAt<=request.end)
                    .GroupBy(a=>a.CreatedAt)
                    .CountAsync();
                  
                return new Response(count);
            }
        }
    }
}
