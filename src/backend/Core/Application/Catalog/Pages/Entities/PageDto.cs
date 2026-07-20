namespace EvrenDev.Application.Catalog.Pages.Entities;

public class PageDto : IDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public Guid ChapterId { get; set; }
    public string? ChapterTitle { get; set; }
    public int Order { get; set; }
    public string? ContentType { get; set; }
    public bool NeedsReview { get; set; }
    public bool IsImported { get; set; }
}
