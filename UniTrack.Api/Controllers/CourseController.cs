using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniTrack.Api.Data;
using UniTrack.Api.Dtos;
using UniTrack.Api.Models;

namespace UniTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetAll(CancellationToken ct) =>
        await db.Courses
            .AsNoTracking()
            .OrderBy(c => c.Code)
            .Select(c => new CourseDto(c.Id, c.Code, c.Name))
            .ToListAsync(ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CourseDto>> GetById(int id, CancellationToken ct)
    {
        var course = await db.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CourseDto(c.Id, c.Code, c.Name))
            .FirstOrDefaultAsync(ct);

        return course is null ? NotFound() : course;
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> Create(CreateCourseRequest request, CancellationToken ct)
    {
        var code = request.Code.Trim().ToUpperInvariant();

        if (await db.Courses.AnyAsync(c => c.Code == code, ct))
            return Conflict($"Course {code} already exists.");

        var course = new Course { Code = code, Name = request.Name.Trim() };
        db.Courses.Add(course);
        await db.SaveChangesAsync(ct);

        var dto = new CourseDto(course.Id, course.Code, course.Name);
        return CreatedAtAction(nameof(GetById), new { id = course.Id }, dto);
    }
}