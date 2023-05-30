using CryptoBankVerticalWebApi.Database;
using CryptoBankVerticalWebApi.Features.Users.Domain;
using CryptoBankVerticalWebApi.Features.Users.Model;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CryptoBankVerticalWebApi.Features.Users.Request
{
    public static class UpdateUserRole
    {
        public record Request(UpdateUserRoleModel updateUserRoleModel) : IRequest<Response>;

        public record Response(UserModel UserModel);

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator(ApplicationDbContext applicationDbContext)
            {
                ClassLevelCascadeMode = CascadeMode.Stop;
                RuleFor(x => x.updateUserRoleModel.Email).NotEmpty().EmailAddress();
                RuleFor(x => x.updateUserRoleModel.UpdatedRole).Must(role => IsValidRole(role))
                    .WithMessage("This role not find");

                RuleFor(x => x.updateUserRoleModel.Email)
                   .NotEmpty()
                   .MustAsync(async (x, token) =>
                   {
                       var isExistUser = await applicationDbContext.Users.AnyAsync(user => user.Email == x, token);

                       return isExistUser;
                   }).WithMessage("User not exists in database");
            }

            private bool IsValidRole(string role)
            {
                foreach (var item in Enum.GetValues(typeof(UserRole)))
                {
                    if (Enum.GetName(typeof(UserRole), item) == role)
                    {
                        return true; // role является допустимой ролью
                    }
                }
                return false;
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
                UserRole updateuserRole = (UserRole)Enum.Parse(typeof(UserRole),request.updateUserRoleModel.UpdatedRole);
                //UserRole updateuserRole = request.updateUserRoleModel.UserRole;
                var user = await _applicationDbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email==request.updateUserRoleModel.Email);
                user = new Domain.User()
                {
                    Id=user.Id,
                    Email=user.Email,
                    BirthDate =user.BirthDate,
                    DateOfRegistration =user.DateOfRegistration,
                    CreatedAt=user.CreatedAt,
                    Password =user.Password,
                    PasswordSalt=user.PasswordSalt,
                    Role = updateuserRole,
                    DeleteAt = user.DeleteAt,
                    UpdatedAt = DateTime.Now.ToUniversalTime(),
                };
                _applicationDbContext.Users.Update(user);
                _applicationDbContext.SaveChanges();
                return new Response(ToUserModel(user));
            }
            private static UserModel ToUserModel(User user)
            {
                return new UserModel()
                {
                    Email = user.Email,
                    UpdatedAt = user.UpdatedAt,
                    UserRole = user.Role,
                    DateOfBirth = user.BirthDate,
                    DateOfRegistration = user.DateOfRegistration,
                };
            }
        }
    }
}
