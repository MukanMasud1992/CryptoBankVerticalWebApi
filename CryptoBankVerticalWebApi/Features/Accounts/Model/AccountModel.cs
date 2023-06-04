using CryptoBankVerticalWebApi.Features.Users.Domain;

namespace CryptoBankVerticalWebApi.Features.Accounts.Model
{
    public class AccountModel
    {
        public Guid Id { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateOfOpening { get; set; }
        public Int64 UserId { get; set; }
    }
}
