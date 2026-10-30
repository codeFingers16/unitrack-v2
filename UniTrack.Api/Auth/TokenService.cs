using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using UniTrack.Api.Dtos;
using UniTrack.Api.Models;

namespace UniTrack.Api.Auth;

public interface ITokenService
{
    AuthResponse CreateToken(AppUser user);
}

public class TokenService(JwtOptions options) : ITokenService
{
    public AuthResponse CreateToken(AppUser user)
    {
        var expires = DateTime.UtcNow.AddMinutes(options.ExpiryMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = expires,
            Issuer = options.Issuer,
            Audience = options.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)),
                SecurityAlgorithms.HmacSha256)
        };

        return new AuthResponse(new JsonWebTokenHandler().CreateToken(descriptor), expires);
    }
}