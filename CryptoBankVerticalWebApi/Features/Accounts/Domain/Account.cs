using CryptoBankVerticalWebApi.Features.Users.Domain;

namespace CryptoBankVerticalWebApi.Features.Accounts.Domain
{
    public class Account
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public long UserId { get; set; }
        public virtual User User { get; set; }
    }
}
