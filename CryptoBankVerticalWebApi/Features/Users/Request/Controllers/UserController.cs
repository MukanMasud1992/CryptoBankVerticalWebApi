using CryptoBankVerticalWebApi.Features.Users.Model;
using CryptoBankVerticalWebApi.Pipeline;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CryptoBankVerticalWebApi.Features.Users.Request.Controllers
{
    [ApiController]
    [Route("/user")]
    public class UserController: Controller
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
        public async Task<RegisterUser.Response> Register(RegisterUser.Request request, CancellationToken cancellationToken) => await _mediator.Send(request, cancellationToken);
 
    }
}
