using CryptoBankVerticalWebApi.Authorization.Requirements;
using CryptoBankVerticalWebApi.Features.Accounts.Model;
using CryptoBankVerticalWebApi.Features.Auth.Model;
using CryptoBankVerticalWebApi.Features.Users.Request;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;
using System.Security.Authentication;

namespace CryptoBankVerticalWebApi.Features.Accounts.Request.Controllers
{
    [ApiController]
    [Route("/account")]
    public class AccountController:Controller
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator) => _mediator = mediator;

        [HttpPost("CreateAccount")]
        //public Task<LoginUser.Response> Authenticate(LoginUser.Request request, CancellationToken cancellationToken) =>
        //    _mediator.Send(request, cancellationToken);

        public async Task<CreateAccount.Response> LoginUser(CreateAccountModel createAccountModel, CancellationToken cancellationToken)
        {
            var request = new CreateAccount.Request(createAccountModel);
            var response = await _mediator.Send(request, cancellationToken);
            return response;
        }
        [HttpGet("GetOwnAccounts")]
        public async Task<GetUserAccounts.Response> GetOwnAccounts(CancellationToken cancellationToken)
        {
            var user = HttpContext?.User;
            long userId = 0;

            if (user!=null && user.Claims.Any())
            {
                var claimUserId = user.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                if (string.IsNullOrEmpty(claimUserId))
                {
                    throw new AuthenticationException("User not exist");
                }

                if (!long.TryParse(claimUserId, out userId))
                {
                    throw new AuthenticationException("Invalid user id");
                }
            }
            var request = new GetUserAccounts.Request(userId);
            var response = await _mediator.Send(request, cancellationToken);
            return response;
        }

        [Authorize(Policy = PolicyNames.AnalystRole)]
        [HttpGet("GetAccountsByPeriod")]
        public async Task<GetAccountsByPeriod.Response> GetAccountsByPeriod([FromBody] DateTime start, DateTime end, CancellationToken cancellationToken)
        {
            
            var request = new GetAccountsByPeriod.Request(start,end);
            var response = await _mediator.Send(request, cancellationToken);
            return response;
        }
    }
}
