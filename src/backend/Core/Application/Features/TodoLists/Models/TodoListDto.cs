namespace EvrenDev.Application.Features.TodoLists.Models;

public class TodoListDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public Colour Colour { get; set; } = Colour.White;
    public List<BasicTodoItemDto> Items { get; set; } = new();
}
