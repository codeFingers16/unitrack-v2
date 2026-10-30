using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace UniTrack.Api.Auth;

public static class UserClaimsExtensions
{
    public static string GetUserId(this ClaimsPrincipal user) =>
        user.FindFirstValue(JwtRegisteredClaimNames.Sub)
        ?? throw new InvalidOperationException("Authenticated user has no 'sub' claim.");
}