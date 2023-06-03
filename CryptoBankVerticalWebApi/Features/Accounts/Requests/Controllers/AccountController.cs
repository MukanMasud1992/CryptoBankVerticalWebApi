using CryptoBankVerticalWebApi.Authorization.Requirements;
using CryptoBankVerticalWebApi.Features.Accounts.Model;
using CryptoBankVerticalWebApi.Features.Accounts.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CryptoBankVerticalWebApi.Features.Accounts.Request.Controllers
{
    [ApiController]
    [Route("/account")]
    public class AccountController:Controller
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator) => _mediator = mediator;

        [Authorize]
        [HttpPost("create-account")]
        public async Task<CreateAccount.Response> CreateAccount(CreateAccountModel createAccountModel, CancellationToken cancellationToken)
        {
            var user = HttpContext?.User;
            long userId = Convert.ToInt64(user.Claims.FirstOrDefault(x => x.Type==ClaimTypes.NameIdentifier)?.Value);
            var request = new CreateAccount.Request(createAccountModel,userId);
            var response = await _mediator.Send(request, cancellationToken);
            return response;
        }

        [Authorize]
        [HttpGet("get-own-accounts")]
        public async Task<GetUserAccounts.Response> GetOwnAccounts(CancellationToken cancellationToken)
        {
            var user = HttpContext?.User;
            long userId = Convert.ToInt64(user.Claims.FirstOrDefault(x => x.Type==ClaimTypes.NameIdentifier)?.Value);
            var request = new GetUserAccounts.Request(userId);
            var response = await _mediator.Send(request, cancellationToken);
            return response;
        }

        [Authorize(Policy = PolicyNames.AnalystRole)]
        [HttpGet("get-accounts-by-period")]
        public async Task<GetAccountsByPeriod.Response> GetAccountsByPeriod([FromQuery] DateTime start, [FromQuery] DateTime end, CancellationToken cancellationToken)
        {
            var request = new GetAccountsByPeriod.Request(start,end);
            var response = await _mediator.Send(request, cancellationToken);
            return response;
        }
    }
}
