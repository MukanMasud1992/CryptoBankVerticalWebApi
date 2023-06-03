using CryptoBankVerticalWebApi.Features.Users.Domain;

namespace CryptoBankVerticalWebApi.Features.Accounts.Model
{
    public class AccountModel
    {
        public long Id { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public long UserId { get; set; }
    }
}
