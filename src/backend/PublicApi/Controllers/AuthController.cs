using EvrenDev.Application.Common.Models;
using EvrenDev.Application.Features.Auth.Commands;
using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EvrenDev.PublicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IMediator _mediator;

    public AuthController(
        SignInManager<ApplicationUser> signInManager,
        IMediator mediator)
    {
        _signInManager = signInManager;
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [Authorize]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponse>> RefreshToken(RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }
}
