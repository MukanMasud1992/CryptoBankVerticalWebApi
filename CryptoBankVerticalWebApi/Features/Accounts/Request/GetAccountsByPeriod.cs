using CryptoBankVerticalWebApi.Database;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CryptoBankVerticalWebApi.Features.Accounts.Request
{
    public class GetAccountsByPeriod
    {
        public record Request(DateTime start,DateTime end):IRequest<Response>;
        public record Response(int count);
        public class RequestValidator:AbstractValidator<Request>
        {
            public RequestValidator(ApplicationDbContext applicationDbContext) 
            {
                ClassLevelCascadeMode = CascadeMode.Stop;
                RuleFor(x => x.start).NotEmpty();
                RuleFor(x=>x.end).NotEmpty();

                RuleFor(x => new { x.start,x.end}).Must(pair => IsValidRange(pair.start, pair.end))
                    .WithMessage("Invalid range");
            }

            private bool IsValidRange(DateTime start, DateTime end)
            {
                if(start>end)
                {
                    return false;
                }
                if (start<end)
                {
                    return true;
                }
                else
                {
                    return true;
                }
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
                    .Where(account => account.DateOfOpening>=request.start && account.DateOfOpening<=request.end)
                    .CountAsync();
                  
                return new Response(count);
            }
        }
    }
}
