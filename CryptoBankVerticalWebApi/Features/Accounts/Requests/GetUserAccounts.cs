﻿using CryptoBankVerticalWebApi.Database;
using CryptoBankVerticalWebApi.Features.Accounts.Model;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CryptoBankVerticalWebApi.Features.Accounts.Requests
{
    public static class GetUserAccounts
    {
        public record Request(long userId) : IRequest<Response>;

        public record Response(List<AccountModel> accountModels);

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator(ApplicationDbContext applicationDbContext)
            {
                RuleFor(x => x.userId)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty();

                RuleFor(x => x.userId)
                    .Cascade(CascadeMode.Stop)
                    .MustAsync(async (x, token) =>
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
                List<AccountModel> accountModels = accounts?.Select(account => new AccountModel()
                {
                    Id = account.Id,
                    UserId = account.UserId,
                    Amount = account.Amount,
                    Currency = account.Currency,
                    CreatedAt = account.CreatedAt
                }).ToList() ?? new List<AccountModel>();
                return new Response(accountModels);
            }
        }
    }
}