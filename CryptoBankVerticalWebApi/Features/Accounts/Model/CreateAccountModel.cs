using CryptoBankVerticalWebApi.Features.Users.Domain;

namespace CryptoBankVerticalWebApi.Features.Accounts.Model
{
    public class CreateAccountModel
    {
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public Int64 UserId { get; set; }
    }
}
