using CryptoBankVerticalWebApi.Features.Auth.Options;
using CryptoBankVerticalWebApi.Features.Users.Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;

namespace CryptoBankVerticalWebApi.Features.Auth.Helpers
{
    public class TokenHelper
    {
        public static async Task<string> GenerateToken(User user)
        {
            var role = (UserRole)user.Role;
            var roleString = role.ToString();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role,roleString)
            };

  

            var keyBytes = Convert.FromBase64String(JWTSettings.JWT.SigningKey);
            var key = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            //var token = new JwtSecurityTokenHandler
            //{
            //    Subject=claims,
            //    Issuer=JWTSettings.JWT.Issuer,
            //    Audience = JWTSettings.JWT.Audience,
            //    Expires=DateTime.UtcNow.AddHours(12),
            //    SigningCredentials = credentials,
            //};
            var expires = DateTime.Now + JWTSettings.JWT.Expiration;
            var token = new JwtSecurityToken(
                JWTSettings.JWT.Issuer,
                JWTSettings.JWT.Audience,
                claims:claims,
                expires: expires,
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
           // var securityToken = tokenHandler.CreateToken(token);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //public static async Task<string> GenerateRefreshToken()
        //{
        //    var randomNumber = new byte[64];
        //    using (var rng = RandomNumberGenerator.Create())
        //    {
        //        rng.GetBytes(randomNumber);
        //        return Convert.ToBase64String(randomNumber);
        //    }
        //}
    }
}
