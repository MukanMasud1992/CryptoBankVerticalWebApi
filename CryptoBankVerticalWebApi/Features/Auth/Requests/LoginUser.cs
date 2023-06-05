using CryptoBankVerticalWebApi.Database;
using CryptoBankVerticalWebApi.Features.Auth.Model;
using CryptoBankVerticalWebApi.Features.Auth.Services;
using CryptoBankVerticalWebApi.Features.Users.Domain;
using CryptoBankVerticalWebApi.Features.Users.Options;
using CryptoBankVerticalWebApi.Features.Users.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CryptoBankVerticalWebApi.Features.Auth.Requests
{
    public static class LoginUser
    {
        public record Request(LoginModel loginModel) : IRequest<Response>;
        public record Response(string jwt);

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator(ApplicationDbContext applicationDbContext)
            {
               
                RuleFor(x => x.loginModel.Email)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .EmailAddress();
                RuleFor(x => x.loginModel.Password)
                      .Cascade(CascadeMode.Stop)
                      .MinimumLength(3);
            }
        }
        public class RequestHandler : IRequestHandler<Request, Response>
        {
            private readonly ApplicationDbContext _applicationDbContext;
            private readonly TokenGenerateService _tokenGenerateService;
            private readonly PasswordHeshingService _passwordHeshingService;
            private readonly PasswordHashingOptions _passwordHasherOptions;

            public RequestHandler(ApplicationDbContext applicationDbContext
                ,TokenGenerateService tokenGenerateService
                ,PasswordHeshingService passwordHeshingService
                ,IOptions<PasswordHashingOptions> passwordHasherOptions
                )
            {
                _applicationDbContext = applicationDbContext;
                _tokenGenerateService=tokenGenerateService;
                _passwordHeshingService=passwordHeshingService;
                _passwordHasherOptions=passwordHasherOptions.Value;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var user = await _applicationDbContext.Users.Include(u => u.Roles).SingleOrDefaultAsync(u => u.Email==request.loginModel.Email, cancellationToken);
                if (user==null)
                {
                    throw new Exception("Invalid credentials");
                }

                if (!_passwordHeshingService.CheckPasswordHasherOptions(user))
                {
                    var updatePasswordSalt = _passwordHeshingService.GenerateSalt();
                    var updatePasswordHash = _passwordHeshingService.GetPasswordHash(request.loginModel.Password, Convert.FromBase64String(updatePasswordSalt));
                    var updatePasswordHashAndSalt = $"{updatePasswordHash}:{updatePasswordSalt}";
                    user.PasswordHashAndSalt = updatePasswordHashAndSalt;
                    user.UpdatedAt = DateTime.Now.ToUniversalTime();
                    user.Parallelism = _passwordHasherOptions.Parallelism;
                    user.Iterations = _passwordHasherOptions.Iterations;
                    user.MemorySize = _passwordHasherOptions.MemorySize;
                    _applicationDbContext.Users.Update(user);
                    await _applicationDbContext.SaveChangesAsync(cancellationToken);;
                }
                string[] parts = user.PasswordHashAndSalt.Split(':');
                string passwordSalt = parts[1];
                var passwordHash = _passwordHeshingService.GetPasswordHash(request.loginModel.Password, Convert.FromBase64String(passwordSalt));
                var passwordHashAndSalt = $"{passwordHash}:{passwordSalt}";

                if (passwordHashAndSalt!=user.PasswordHashAndSalt)
                {
                    throw new Exception("Invalid credentials");
                }

                var jwt = await _tokenGenerateService.GenerateToken(user);
                return new Response(jwt);
            }
        }
    }
}
