using BuberDinner.Application.Services.Authentication;
using BuberDinner.Contracts.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace BuberDinner.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticatitonService;

    public AuthenticationController(IAuthenticationService authenticatitonService)
    {
        _authenticatitonService = authenticatitonService;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var authResult = _authenticatitonService.Login(request.Email, request.Password);

        return Ok();
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        return Ok();
    }
}