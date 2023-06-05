using CryptoBankVerticalWebApi.Features.Users.Domain;

namespace CryptoBankVerticalWebApi.Features.Auth.Model;

public class RefreshToken
{
    public long Id { get; set; }
    public string Token { get; set; }
    public long userId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool Revoke { get; set; }
    public long ReplacedByNextToken { get; set; }

    public virtual User User { get; set; }
}
