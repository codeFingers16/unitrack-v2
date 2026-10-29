using System.ComponentModel.DataAnnotations;

namespace UniTrack.Api.Dtos;

public record CourseDto(int Id, string Code, string Name);

public class CreateCourseRequest
{
    [Required, StringLength(20)]
    public string Code { get; init; } = string.Empty;

    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;
}