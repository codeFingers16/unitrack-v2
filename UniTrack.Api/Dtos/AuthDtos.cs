using System.ComponentModel.DataAnnotations;

namespace UniTrack.Api.Dtos;

public class RegisterRequest
{
    [Required, EmailAddress, StringLength(256)] public string Email { get; init; } = string.Empty;
    [Required, StringLength(128)] public string Password { get; init; } = string.Empty;
}

public class LoginRequest
{
    [Required, EmailAddress, StringLength(256)] public string Email { get; init; } = string.Empty;
    [Required, StringLength(128)] public string Password { get; init; } = string.Empty;
}

public record AuthResponse(string AccessToken, DateTime ExpiresAt);