using CryptoBankVerticalWebApi.Features.Users.Domain;

namespace CryptoBankVerticalWebApi.Features.Users.Model
{
    public class UserModel
    {
        public string Email { get; set; }

        public DateTime DateOfRegistration { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime DateOfBirth { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserRole? UserRole { get; set; }
    }
}
