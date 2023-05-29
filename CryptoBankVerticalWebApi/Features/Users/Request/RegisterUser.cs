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
        public record Request(string email,string password,DateTime birthDate) : IRequest<Response>;
        public record Response(string result);

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.email).NotEmpty().EmailAddress();
                RuleFor(x => x.password).NotEmpty();
                RuleFor(x => x.birthDate).Must(date => IsValidBirthDateMoreThenEightTeen(date)).Must(date=>IsValidBirthDateLessThenHundred(date));
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
                try
                {
                    var passwordSalt = PasswordHelper.GenerateSalt();
                    var passwordHash = PasswordHelper.GetPasswordHash(request.password, passwordSalt);
                    if (request.email.Equals(_usersOptions.Value.AdministratorEmail))
                    {
                        var admin = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Email==request.email);
                        if (admin!=null)
                        {
                            return new Response("the admin email is already in the system");
                        }
                        admin = new User()
                        {
                            Email = request.email,
                            PasswordSalt= PasswordHelper.GenerateSalt(),
                            Password = passwordHash,
                            BirthDate = request.birthDate.ToUniversalTime(),
                            CreatedAt = DateTime.Now.ToUniversalTime(),
                            DateOfRegistration = DateTime.Now.ToUniversalTime(),
                            Role = UserRole.Administrator,

                        };
                        _applicationDbContext.Users.Add(admin);
                        await _applicationDbContext.SaveChangesAsync();
                        return new Response("Administrator register succes");
                    }
                    var user = await _applicationDbContext.Users.FirstOrDefaultAsync(u=>u.Email==request.email);
           
                    if (user!=null)
                    {
                        return new Response("the user email is already in the system");
                    }
                    user = new User()
                    {
                        Email = request.email,
                        PasswordSalt= PasswordHelper.GenerateSalt(),
                        Password = passwordHash,
                        BirthDate = request.birthDate.ToUniversalTime(),
                        CreatedAt = DateTime.Now.ToUniversalTime(),
                        DateOfRegistration = DateTime.Now.ToUniversalTime(),
                        Role = UserRole.User,
                    };

                    _applicationDbContext.Users.Add(user);
                    await _applicationDbContext.SaveChangesAsync();
                    return new Response("User register succes");
                }
                catch (Exception ex)
                {
                    return new Response($"User register unsuccessfully + {ex.Message}");
                }
            }
        }

        
    }
}
