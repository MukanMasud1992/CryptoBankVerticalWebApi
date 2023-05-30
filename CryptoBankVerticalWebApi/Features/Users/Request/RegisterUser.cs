using CryptoBankVerticalWebApi.Database;
using CryptoBankVerticalWebApi.Features.Users.Domain;
using CryptoBankVerticalWebApi.Features.Users.Helpers;
using CryptoBankVerticalWebApi.Features.Users.Model;
using CryptoBankVerticalWebApi.Features.Users.Options;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;

namespace CryptoBankVerticalWebApi.Features.Users.Request
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
                ClassLevelCascadeMode = CascadeMode.Stop;
                RuleFor(x => x.registerUserModel.Email)
                    .NotEmpty()
                    .EmailAddress();
                RuleFor(x => x.registerUserModel.Password)
                .MinimumLength(3);

                RuleFor(x => x.registerUserModel.BirthDate)
                    .NotEmpty()
                    .Must(date=>IsValidBirthDateLessThenHundred(date))
                    .Must(date=>IsValidBirthDateMoreThenEightTeen(date));

                RuleFor(x => x.registerUserModel.Email).MustAsync(async (x, token) =>
                {
                    var isExistUser = await applicationDbContext.Users.AnyAsync(user => user.Email == x);

                    return !isExistUser;
                }).WithMessage("User already in system");
            }

            private bool IsValidBirthDateMoreThenEightTeen(DateTime birthDate)
            {
                var currentDate = DateTime.UtcNow;
                var minDate = currentDate.AddYears(-18);
                return birthDate <= minDate;
            }
            private bool IsValidBirthDateLessThenHundred(DateTime birthDate)
            {
                var currentDate = DateTime.UtcNow;
                var minDate = currentDate.AddYears(+100);
                return birthDate <= minDate;
            }
        }

        public class RequestHandler : IRequestHandler<Request, Response>
        {
            private readonly ApplicationDbContext _applicationDbContext;
            private readonly IOptions<UsersOptions> _usersOptions;

            public RequestHandler(ApplicationDbContext applicationDbContext,IOptions<UsersOptions> usersOptions)
            {
                _applicationDbContext = applicationDbContext;
                _usersOptions=usersOptions;
            }
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var passwordSalt = PasswordHelper.GenerateSalt();
                var passwordHash = PasswordHelper.GetPasswordHash(request.registerUserModel.Password, Convert.FromBase64String(passwordSalt));
                if (request.registerUserModel.Email.Equals(_usersOptions.Value.AdministratorEmail))
                {
                    var admin = new User()
                    {
                        Email = request.registerUserModel.Email,
                        PasswordSalt= passwordSalt,
                        Password = passwordHash,
                        BirthDate = request.registerUserModel.BirthDate.ToUniversalTime(),
                        CreatedAt = DateTime.Now.ToUniversalTime(),
                        DateOfRegistration = DateTime.Now.ToUniversalTime(),
                        Role = UserRole.AdministratorRole,

                    };
                    _applicationDbContext.Users.Add(admin);
                    await _applicationDbContext.SaveChangesAsync();
                    return new Response(ToUserModel(admin));
                }
                var user = new User()
                {
                    Email = request.registerUserModel.Email,
                    PasswordSalt= passwordSalt,
                    Password = passwordHash,
                    BirthDate = request.registerUserModel.BirthDate.ToUniversalTime(),
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                    DateOfRegistration = DateTime.Now.ToUniversalTime(),
                    Role = UserRole.UserRole,
                };

                _applicationDbContext.Users.Add(user);
                await _applicationDbContext.SaveChangesAsync();
                return new Response(ToUserModel(user));
            }
            private static UserModel ToUserModel(User user)
            {
                return new UserModel()
                {
                    Email = user.Email,
                    DateOfBirth = user.BirthDate,
                    DateOfRegistration = user.DateOfRegistration,
                    UserRole = user.Role
                };
            }
        }     
    }
}
