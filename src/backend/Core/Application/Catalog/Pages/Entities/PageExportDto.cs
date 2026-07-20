namespace EvrenDev.Application.Catalog.Pages.Entities;

public class PageExportDto : IDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string ChapterTitle { get; set; } = default!;
    public int Order { get; set; }
}
