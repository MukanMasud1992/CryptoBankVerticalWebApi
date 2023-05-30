namespace CryptoBankVerticalWebApi.Features.Users.Model
{
    public class RegisterUserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
