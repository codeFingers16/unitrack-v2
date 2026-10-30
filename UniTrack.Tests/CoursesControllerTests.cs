using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniTrack.Api.Controllers;
using UniTrack.Api.Data;
using UniTrack.Api.Dtos;
using UniTrack.Api.Models;

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

    private static CoursesController CreateController(AppDbContext db, string userId)
    {
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(new[] { new Claim("sub", userId) }, "test"));

        return new CoursesController(db)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            }
        };
    }

    [Fact]
    public async Task Create_ValidCourse_NormalisesCodeAndAssignsCurrentUser()
    {
        await using var db = CreateDb();
        var controller = CreateController(db, "user-1");

        var result = await controller.Create(
            new CreateCourseRequest { Code = " cos1511 ", Name = "Introduction to Programming" },
            CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<CourseDto>(created.Value);
        Assert.Equal("COS1511", dto.Code);
        Assert.Equal("user-1", (await db.Courses.SingleAsync()).UserId);
    }

    [Fact]
    public async Task GetById_CourseOwnedByAnotherUser_ReturnsNotFound()
    {
        await using var db = CreateDb();
        db.Courses.Add(new Course { Code = "COS1511", Name = "Intro", UserId = "user-1" });
        await db.SaveChangesAsync();
        var id = await db.Courses.Select(c => c.Id).SingleAsync();

        var controller = CreateController(db, "user-2");
        var result = await controller.GetById(id, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }
}