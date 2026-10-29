namespace UniTrack.Api.Models;

public class Course
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;   // e.g. COS1511
    public string Name { get; set; } = string.Empty;
    public List<Assignment> Assignments { get; set; } = [];
}