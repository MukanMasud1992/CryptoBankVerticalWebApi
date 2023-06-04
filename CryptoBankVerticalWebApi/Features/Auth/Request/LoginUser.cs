using CryptoBankVerticalWebApi.Database;
using CryptoBankVerticalWebApi.Features.Auth.Helpers;
using CryptoBankVerticalWebApi.Features.Auth.Model;
using CryptoBankVerticalWebApi.Features.Users.Helpers;
using CryptoBankVerticalWebApi.Features.Users.Options;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using static CryptoBankVerticalWebApi.Features.Users.Request.RegisterUser;

namespace CryptoBankVerticalWebApi.Features.Auth.Request
{
    public static class LoginUser
    {
        public record Request(LoginModel loginModel) : IRequest<Response>;
        public record Response(string jwt);

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator(ApplicationDbContext applicationDbContext)
            {
                ClassLevelCascadeMode = CascadeMode.Stop;
                RuleFor(x => x.loginModel.Email)
                    .NotEmpty()
                    .EmailAddress();
                RuleFor(x => x.loginModel.Password)
                .MinimumLength(3);

                RuleFor(x => x.loginModel.Email).MustAsync(async (x, token) =>
                {
                    var isExistUser = await applicationDbContext.Users.AnyAsync(user => user.Email == x);

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
                var user = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Email==request.loginModel.Email);
                if(user==null)
                {
                    return new Response("this email don't register in this system");
                }
                var passwordHash = PasswordHelper.GetPasswordHash(request.loginModel.Password,Convert.FromBase64String(user.PasswordSalt));
                if (passwordHash!=user.Password)
                {
                    return new Response("this email or password wrong");
                }
                var jwt = await TokenHelper.GenerateToken(user);
                return new Response(jwt);
            }
        }
    }
}
