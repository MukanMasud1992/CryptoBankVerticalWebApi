using CryptoBankVerticalWebApi.Features.Users.Domain;

namespace CryptoBankVerticalWebApi.Features.Users.Model
{
    public class UserModel
    {
        public Int64 Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public DateTime DateOfRegistration { get; set; } = DateTime.Now.ToUniversalTime();
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeleteAt { get; set; }
    }
}
