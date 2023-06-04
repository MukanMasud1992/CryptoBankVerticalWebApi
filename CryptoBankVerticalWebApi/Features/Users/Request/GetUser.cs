using CryptoBankVerticalWebApi.Database;
using CryptoBankVerticalWebApi.Features.Users.Model;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace CryptoBankVerticalWebApi.Features.Users.Request
{
    public static class GetUser
    {
        public record Request(long userId) : IRequest<Response>;

        public record Response(UserModel userModel);

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator(ApplicationDbContext applicationDbContext)
            {
                ClassLevelCascadeMode = CascadeMode.Stop;
                RuleFor(x => x.userId)
                    .NotEmpty()
                    .MustAsync(async (x, token) =>
                    {
                        var isExistUser = await applicationDbContext.Users.AnyAsync(user => user.Id == x, token);

                        return isExistUser;
                    }).WithMessage("User not exists in database");
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
                var user = await _applicationDbContext.Users.FindAsync(request.userId);

                return new Response(new UserModel()
                {
                    Email = user.Email,
                    DateOfBirth = user.BirthDate,
                    DateOfRegistration = user.DateOfRegistration,
                    UserRole = user.Role,
                    UpdatedAt = null
                });
            }
        }
    }
}
