using Microsoft.AspNetCore.Identity;

namespace UniTrack.Api.Models;

public class AppUser : IdentityUser
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}