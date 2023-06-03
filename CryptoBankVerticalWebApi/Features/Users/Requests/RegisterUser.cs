using CryptoBankVerticalWebApi.Database;
using CryptoBankVerticalWebApi.Features.Users.Domain;
using CryptoBankVerticalWebApi.Features.Users.Model;
using CryptoBankVerticalWebApi.Features.Users.Options;
using CryptoBankVerticalWebApi.Features.Users.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;

namespace CryptoBankVerticalWebApi.Features.Users.Requests
{
    public static class RegisterUser
    {
        //public record Request(string email, string password, DateTime birthDate): IRequest<Response>;
        public record Request( RegisterUserModel registerUserModel) : IRequest<Response>;
        public record Response(UserModel UserModel);

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator(ApplicationDbContext applicationDbContext)
            {
               
                RuleFor(x => x.registerUserModel.Email)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .EmailAddress();
                RuleFor(x => x.registerUserModel.Password)
                .MinimumLength(3);

                RuleFor(x => x.registerUserModel.BirthDate)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .Must(date=>IsValidBirthDateMoreThenEightTeen(date));

                RuleFor(x => x.registerUserModel.Email)
                    .Cascade(CascadeMode.Stop)
                    .MustAsync(async (x, token) =>
                {
                    var isExistUser = await applicationDbContext.Users.AnyAsync(user => user.Email == x,token);

                    return !isExistUser;
                }).WithMessage("User already in system");
            }

            private bool IsValidBirthDateMoreThenEightTeen(DateTime birthDate)
            {
                var currentDate = DateTime.UtcNow;
                var minDate = currentDate.AddYears(-18);
                return birthDate <= minDate;
            }
        }

        public class RequestHandler : IRequestHandler<Request, Response>
        {
            private readonly ApplicationDbContext _applicationDbContext;
            private readonly PasswordHeshingService _passwordHeshingService;
            private readonly UsersOptions _usersOptions;
            private readonly PasswordHashingOptions _passwordHashOptions;

            public RequestHandler(ApplicationDbContext applicationDbContext
                ,PasswordHeshingService passwordHeshingService
                ,IOptions<UsersOptions> usersOptions
                ,IOptions<PasswordHashingOptions> passwordHashOptions)
            {
                _applicationDbContext = applicationDbContext;
                _passwordHeshingService=passwordHeshingService;
                _usersOptions=usersOptions.Value;
                _passwordHashOptions=passwordHashOptions.Value;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                UserRole userRole = UserRole.UserRole;
                var passwordSalt = _passwordHeshingService.GenerateSalt();
                var passwordHash = _passwordHeshingService.GetPasswordHash(request.registerUserModel.Password, Convert.FromBase64String(passwordSalt));
                var passwordHashAndSalt = $"{passwordHash}:{passwordSalt}";
                if (request.registerUserModel.Email.Equals(_usersOptions.AdministratorEmail))
                {
                    userRole = UserRole.AdministratorRole;
                }
                var user = new User()
                {
                    Email = request.registerUserModel.Email,
                    PasswordHashAndSalt = passwordHashAndSalt,
                    Parallelism = _passwordHashOptions.Parallelism,
                    Iterations = _passwordHashOptions.Iterations,
                    MemorySize = _passwordHashOptions.MemorySize,
                    BirthDate = request.registerUserModel.BirthDate.ToUniversalTime(),
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                    Roles = new List<Role>()
                    {
                        new Role()
                        {
                            Name = userRole,
                            CreatedAt = DateTime.Now.ToUniversalTime()
                        }
                    }
                };
                await _applicationDbContext.Users.AddAsync(user, cancellationToken);
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
                return new Response(ToUserModel(user));
            }

            private static UserModel ToUserModel(User user)
            {
                return new UserModel()
                {

                    Email = user.Email,
                    DateOfBirth = user.BirthDate,
                    DateOfRegistration = user.CreatedAt,
                    Roles = user.Roles.Select(role => new RoleModel
                    {
                        RoleName = role.Name.ToString(),
                        CreatedAt = role.CreatedAt
                    }).ToList()
                };
            }
        }     
    }
}
