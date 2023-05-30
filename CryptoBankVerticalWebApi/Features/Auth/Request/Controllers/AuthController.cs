using CryptoBankVerticalWebApi.Authorization.Requirements;
using CryptoBankVerticalWebApi.Features.Auth.Model;
using CryptoBankVerticalWebApi.Features.Users.Domain;
using CryptoBankVerticalWebApi.Features.Users.Request;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace CryptoBankVerticalWebApi.Features.Auth.Request.Controllers
{
    [ApiController]
    [Route("/auth")]
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("Login")]
        //public Task<LoginUser.Response> Authenticate(LoginUser.Request request, CancellationToken cancellationToken) =>
        //    _mediator.Send(request, cancellationToken);

        public async Task<LoginUser.Response> LoginUser(LoginModel loginModel, CancellationToken cancellationToken)
        {
            var request = new LoginUser.Request(loginModel);
            var response = await _mediator.Send(request, cancellationToken);
            return response;
        }

    }
}
