using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Api.Controllers;
using UniTrack.Api.Data;
using UniTrack.Api.Dtos;

namespace UniTrack.Tests;

public class CoursesControllerTests
{
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Create_ValidCourse_PersistsNormalisedCodeAndReturnsCreated()
    {
        await using var db = CreateDb();
        var controller = new CoursesController(db);

        var result = await controller.Create(
            new CreateCourseRequest { Code = " cos1511 ", Name = "Introduction to Programming" },
            CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<CourseDto>(created.Value);
        Assert.Equal("COS1511", dto.Code);
        Assert.Equal(1, await db.Courses.CountAsync());
    }
}