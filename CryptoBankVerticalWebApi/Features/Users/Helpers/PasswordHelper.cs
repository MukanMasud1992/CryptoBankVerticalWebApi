using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace CryptoBankVerticalWebApi.Features.Users.Helpers
{
    public class PasswordHelper
    {
        //public static string GetPasswordHash(string password, byte[] passwordSalt)
        //{
        //    using (var hmac = new HMACSHA256(passwordSalt))
        //    {
        //        var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        //        return Convert.ToBase64String(passwordHash);
        //    }
        //}

        //public static string GetPasswordSalt()
        //{
        //    using (var hmac = new HMACSHA256())
        //    {
        //        var passwordSalt = hmac.Key;
        //        return Convert.ToBase64String(passwordSalt);
        //    }
        //}

        public static string GetPasswordHash(string password, byte[] passwordSalt)
        {


            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {   DegreeOfParallelism = 8,
                MemorySize = 65536,
                Iterations = 4,
                Salt = passwordSalt
            })
            {
                byte[] passwordHash = argon2.GetBytes(32);
                return (Convert.ToBase64String(passwordHash));
            }
            
          
        }

        public static string GenerateSalt()
        {
            // Генерация случайной соли длиной 32 байт
            byte[] salt = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }
     
    }
}
