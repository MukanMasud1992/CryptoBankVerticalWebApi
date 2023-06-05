using CryptoBankVerticalWebApi.Database;
using CryptoBankVerticalWebApi.Features.Auth;
using CryptoBankVerticalWebApi.Features.Auth.Model;
using CryptoBankVerticalWebApi.Features.Auth.Options;
using CryptoBankVerticalWebApi.Features.Users.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CryptoBankVerticalWebApi.Features.Auth.Services
{
    public class TokenGenerateService
    {
        private readonly AuthOptions _authOptions;
        private readonly RefreshTokenOptions _refreshTokenOptions;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly TokenHelper _tokenHelper;

        public TokenGenerateService(IOptions<AuthOptions> authOptions
            ,IOptions<RefreshTokenOptions> refreshTokenOptions
            ,ApplicationDbContext applicationDbContext
            ,TokenHelper tokenHelper) 
        {
            _authOptions=authOptions.Value;
            _refreshTokenOptions=refreshTokenOptions.Value;
            _applicationDbContext=applicationDbContext;
            _tokenHelper=tokenHelper;
        }
        public async Task<(string accessToken,string refreshToken)> GenerateTokensAsync(User user,CancellationToken cancellationToken)
        {
            var refreshTokens = user.RefreshTokens
                .OrderByDescending(x => x.CreatedAt)
                .ToArray();

            var accessToken = _tokenHelper.GenerateAccesToken(user);
            var refreshToken = await _tokenHelper.GenerateRefreshToken();

            await using var transaction = await _applicationDbContext.Database.BeginTransactionAsync(cancellationToken);
            {
                try
                {
                    var newRefreshToken = new RefreshToken
                    {
                        userId = user.Id,
                        Token = refreshToken,
                        ExpiryDate = DateTime.Now.Add(_refreshTokenOptions.RefreshTokenExpiration).ToUniversalTime(),
                        CreatedAt = DateTime.Now.ToUniversalTime(),
                    };

                    user.RefreshTokens.Add(newRefreshToken);
                    _applicationDbContext.Add(newRefreshToken);
                    await _applicationDbContext.SaveChangesAsync(cancellationToken);

                    var NotRevokeRefreshToken = refreshTokens.FirstOrDefault(t => !t.Revoke);
                    if (NotRevokeRefreshToken!=null)
                    {
                        NotRevokeRefreshToken.Revoke = true;
                        NotRevokeRefreshToken.ReplacedByNextToken=newRefreshToken.Id;
                    }

                    var overdueTokens = refreshTokens.Where(x => x.ExpiryDate<=DateTime.Now.ToUniversalTime()).ToArray();

                    _applicationDbContext.RefreshTokens.RemoveRange(overdueTokens);

                    await _applicationDbContext.SaveChangesAsync(cancellationToken);

                    await transaction.CommitAsync(cancellationToken);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }
                return (accessToken, refreshToken);
            }
        }
    }
}
