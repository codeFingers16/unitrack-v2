using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniTrack.Api.Auth;
using UniTrack.Api.Data;
using UniTrack.Api.Dtos;
using UniTrack.Api.Models;

namespace UniTrack.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CoursesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetAll(CancellationToken ct)
    {
        var userId = User.GetUserId();
        return await db.Courses
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Code)
            .Select(c => new CourseDto(c.Id, c.Code, c.Name))
            .ToListAsync(ct);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CourseDto>> GetById(int id, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var course = await db.Courses
            .AsNoTracking()
            .Where(c => c.Id == id && c.UserId == userId)
            .Select(c => new CourseDto(c.Id, c.Code, c.Name))
            .FirstOrDefaultAsync(ct);

        return course is null ? NotFound() : course;
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> Create(CreateCourseRequest request, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var code = request.Code.Trim().ToUpperInvariant();

        if (await db.Courses.AnyAsync(c => c.UserId == userId && c.Code == code, ct))
            return Conflict($"Course {code} already exists.");

        var course = new Course { Code = code, Name = request.Name.Trim(), UserId = userId };
        db.Courses.Add(course);
        await db.SaveChangesAsync(ct);

        var dto = new CourseDto(course.Id, course.Code, course.Name);
        return CreatedAtAction(nameof(GetById), new { id = course.Id }, dto);
    }
}