using CryptoBankVerticalWebApi.Authorization.Requirements;
using CryptoBankVerticalWebApi.Features.Users.Model;
using CryptoBankVerticalWebApi.Features.Users.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CryptoBankVerticalWebApi.Features.Users.Request.Controllers
{
    [ApiController]
    [Route("/user")]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator=mediator;
        }

        [HttpPost("register")]
        public async Task<RegisterUser.Response> Register(RegisterUserModel registerUserModel, CancellationToken cancellationToken)
        {
            var request = new RegisterUser.Request(registerUserModel);
            var response = await _mediator.Send(request, cancellationToken);
            return response;
        }

        [Authorize]
        [HttpGet("get-user")]
        public async Task<GetUser.Response> GetUser(CancellationToken cancellationToken)
        {
            var user = HttpContext?.User;
            long userId = Convert.ToInt64(user.Claims.FirstOrDefault(x => x.Type==ClaimTypes.NameIdentifier)?.Value);
            var request = new GetUser.Request(userId);
            var response = await _mediator.Send(request, cancellationToken);
            return response;
        }

        [Authorize(Policy = PolicyNames.AdministratorRole)]
        [HttpPost("update-user-role")]
        public async Task<UpdateUserRole.Response> UpdateUserRole(UpdateUserRoleModel updateUserRoleModel, CancellationToken cancellationToken)
        {
            var request = new UpdateUserRole.Request(updateUserRoleModel);
            var response = await _mediator.Send(request, cancellationToken);
            return response;
        }




    }
}
