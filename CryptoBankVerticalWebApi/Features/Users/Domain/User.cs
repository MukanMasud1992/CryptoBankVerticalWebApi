using CryptoBankVerticalWebApi.Features.Accounts.Domain;
using CryptoBankVerticalWebApi.Features.Auth.Model;

namespace CryptoBankVerticalWebApi.Features.Users.Domain
{
    public class User
    {
        public User()
        {
            Accounts = new HashSet<Account>();
            Roles = new HashSet<Role>();
            RefreshTokens = new HashSet<RefreshToken>();
        }
        public long Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHashAndSalt { get; set; } = string.Empty;
        public int MemorySize { get; set; }
        public int Iterations { get; set; }
        public int Parallelism { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}
