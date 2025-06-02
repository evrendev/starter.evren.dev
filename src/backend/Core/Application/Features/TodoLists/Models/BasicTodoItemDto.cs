namespace EvrenDev.Application.Features.TodoLists.Models;

public class BasicTodoItemDto
{
    public string? Title { get; set; }
    public string? Note { get; set; }
    public PriorityLevel Priority { get; set; }
    public DateTime? Reminder { get; set; }
    public bool Done { get; set; }
}
