using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using UniTrack.Api.Auth;
using UniTrack.Api.Dtos;
using UniTrack.Api.Models;

namespace UniTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserManager<AppUser> users, ITokenService tokens) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = new AppUser { UserName = request.Email, Email = request.Email };
        var result = await users.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(error.Code, error.Description);
            return ValidationProblem(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await users.FindByEmailAsync(request.Email);
        if (user is null) return Unauthorized();   // same response as a wrong password

        if (await users.IsLockedOutAsync(user))
            return Problem(statusCode: StatusCodes.Status429TooManyRequests,
                           title: "Too many failed attempts. Try again later.");

        if (!await users.CheckPasswordAsync(user, request.Password))
        {
            await users.AccessFailedAsync(user);
            return Unauthorized();
        }

        await users.ResetAccessFailedCountAsync(user);
        return tokens.CreateToken(user);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<object> Me() => new
    {
        Id = User.GetUserId(),
        Email = User.FindFirstValue(JwtRegisteredClaimNames.Email)
    };
}