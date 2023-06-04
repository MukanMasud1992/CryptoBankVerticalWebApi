using CryptoBankVerticalWebApi.Features.Users.Domain;

namespace CryptoBankVerticalWebApi.Features.Users.Model
{
    public class UpdateUserRoleModel
    {
        public string Email { get; set; }
        public string UpdatedRole { get; set; }
    }
}
