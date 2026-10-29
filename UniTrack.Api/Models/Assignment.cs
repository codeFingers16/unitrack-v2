namespace UniTrack.Api.Models;

public enum AssignmentStatus
{
    NotStarted,
    InProgress,
    Submitted
}

public class Assignment
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public DateTimeOffset DueDate { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.NotStarted;
}