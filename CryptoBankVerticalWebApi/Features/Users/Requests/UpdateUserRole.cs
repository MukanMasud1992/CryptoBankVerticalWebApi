using CryptoBankVerticalWebApi.Database;
using CryptoBankVerticalWebApi.Features.Users.Domain;
using CryptoBankVerticalWebApi.Features.Users.Model;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CryptoBankVerticalWebApi.Features.Users.Requests
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
                RuleFor(x => x.updateUserRoleModel.Email)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .EmailAddress();
                RuleFor(x => x.updateUserRoleModel.UpdatedRole)
                    .Cascade(CascadeMode.Stop)
                    .Must(role => IsValidRole(role))
                    .WithMessage("This role not found");

                RuleFor(x => x.updateUserRoleModel.Email)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty();
            }

            private bool IsValidRole(string role)
            {
                foreach (var item in Enum.GetValues(typeof(UserRole)))
                {
                    if (Enum.GetName(typeof(UserRole), item) == role)
                    {
                        return true;
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
                UserRole updateuserRole = (UserRole)Enum.Parse(typeof(UserRole), request.updateUserRoleModel.UpdatedRole);
                var user = await _applicationDbContext.Users
                    .Include(u => u.Roles)
                    .SingleOrDefaultAsync(u => u.Email==request.updateUserRoleModel.Email);
                if (user==null)
                {
                    throw new Exception("Invalid credentials");
                }
                var role = user.Roles.SingleOrDefault(r => r.Name == updateuserRole);
                if (role != null)
                {
                    throw new Exception("User already has this role");
                }

                var newRole = new Role
                {
                    UserId = user.Id,
                    Name = updateuserRole,
                    CreatedAt = DateTime.Now.ToUniversalTime()
                };

                user.Roles.Add(newRole);

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
