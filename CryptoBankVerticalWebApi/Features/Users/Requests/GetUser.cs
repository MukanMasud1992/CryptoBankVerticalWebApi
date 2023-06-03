using CryptoBankVerticalWebApi.Database;
using CryptoBankVerticalWebApi.Features.Users.Model;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace CryptoBankVerticalWebApi.Features.Users.Requests
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
                RuleFor(x => x.userId);
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
                var user = await _applicationDbContext.Users.Include(u => u.Roles).SingleOrDefaultAsync(u => u.Id==request.userId,cancellationToken);
                if(user==null)
                {
                    throw new Exception("User not found");
                }
                return new Response(new UserModel()
                {
                    Email = user.Email,
                    DateOfBirth = user.BirthDate,
                    DateOfRegistration = user.CreatedAt,
                    Roles = user.Roles.Select(role => new RoleModel
                    {
                        RoleName = role.Name.ToString(),
                        CreatedAt = role.CreatedAt
                    }).ToList()
                }); 
            }
        }
    }
}
