namespace EvrenDev.Application.Catalog.Categories.Entities;

public class CategoryDto : IDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
}
