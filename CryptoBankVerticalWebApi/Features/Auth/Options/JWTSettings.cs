using static CryptoBankVerticalWebApi.Features.Auth.Options.JWTSettings;

namespace CryptoBankVerticalWebApi.Features.Auth.Options
{
    public class JWTSettings
    {
        public const string JWTSectionName = "Features:Auth:JWTSettings";
        public static JwtOptions JWT { get; set; } = new JwtOptions();
       
    }
    public class JwtOptions
    {
        public string SigningKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public TimeSpan Expiration { get; set; }
    }
}
