using CryptoBankVerticalWebApi.Features.Auth.Model;
using CryptoBankVerticalWebApi.Features.Auth.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CryptoBankVerticalWebApi.Features.Auth.Request.Controllers
{
    [ApiController]
    [Route("/auth")]
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("login")]
        public async Task<LoginUser.Response> LoginUser(LoginModel loginModel, CancellationToken cancellationToken)
        {
            var request = new LoginUser.Request(loginModel);
            var response = await _mediator.Send(request, cancellationToken);
            return response;
        }

    }
}
