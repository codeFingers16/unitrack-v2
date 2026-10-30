using System.ComponentModel.DataAnnotations;

namespace UniTrack.Api.Auth;

public class JwtOptions
{
    [Required, MinLength(32)] public string Key { get; init; } = string.Empty;
    [Required] public string Issuer { get; init; } = string.Empty;
    [Required] public string Audience { get; init; } = string.Empty;
    [Range(1, 1440)] public int ExpiryMinutes { get; init; } = 60;
}