using CryptoBankVerticalWebApi.Features.Users.Domain;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using static CryptoBankVerticalWebApi.Features.Auth.AuthOptions;

namespace CryptoBankVerticalWebApi.Features.Auth.Services
{
    public class TokenHelper
    {
        private readonly AuthOptions _authOptions;

        public TokenHelper(IOptions<AuthOptions> authOptions)
        {
            _authOptions = authOptions.Value;
        }

        public string GenerateAccesToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Email,user.Email),
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name.ToString()));
            }
            var keyBytes = Convert.FromBase64String(_authOptions.Jwt.SigningKey);
            var key = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var expires = DateTime.Now + _authOptions.Jwt.Expiration;
            var token = new JwtSecurityToken(
                _authOptions.Jwt.Issuer,
                _authOptions.Jwt.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
}
