using CryptoBankVerticalWebApi.Authorization.Requirements;
using CryptoBankVerticalWebApi.Features.Users.Model;
using CryptoBankVerticalWebApi.Pipeline;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace CryptoBankVerticalWebApi.Features.Users.Request.Controllers
{
    [ApiController]
    [Route("/user")]
    public class UserController : Controller
    {
        //private readonly Dispatcher _dispatcher;
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            //_dispatcher = dispatcher;
            _mediator=mediator;
        }

        //[HttpPost("register")]
        //public async Task<string> Register([FromBody] UserModel userModel, CancellationToken cancellationToken)
        //{
        //    var response = await _dispatcher.Dispatch(new RegisterUser.Request(userModel.Email,userModel.Password, userModel.BirthDate),cancellationToken);
        //    return response.result;
        //}
        [HttpPost("register")]
        //public async Task<RegisterUser.Response> Register(RegisterUser.Request request, CancellationToken cancellationToken) =>
        //    await _mediator.Send(request, cancellationToken);
        public async Task<RegisterUser.Response> Register(RegisterUserModel registerUserModel, CancellationToken cancellationToken)
        {
            var request = new RegisterUser.Request(registerUserModel);
            var response = await _mediator.Send(request, cancellationToken);
            return response;
        }

        [HttpGet("GetUser")]
        public async Task<GetUser.Response> GetUser(CancellationToken cancellationToken)
        {
            var user = HttpContext?.User;
            long userId = 0;

            if(user!=null && user.Claims.Any())
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
            var request = new GetUser.Request(userId);
            var response = await _mediator.Send(request, cancellationToken);
            return response;
        }


        //public Task<GetUser.Response> GetUser(GetUser.Request request, CancellationToken cancellationToken) =>
        // _mediator.Send(request, cancellationToken);

        [Authorize(Policy = PolicyNames.AdministratorRole)]
        [HttpPost("UpdateUserRole")]
        public async Task<UpdateUserRole.Response> UpdateUserRole(UpdateUserRoleModel updateUserRoleModel, CancellationToken cancellationToken)
        {
            var request = new UpdateUserRole.Request(updateUserRoleModel);
            var response = await _mediator.Send(request, cancellationToken);
            return response;
        }




    }
}
