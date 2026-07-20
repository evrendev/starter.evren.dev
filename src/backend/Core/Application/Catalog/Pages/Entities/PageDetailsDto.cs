using EvrenDev.Application.Catalog.Chapters.Entities;

namespace EvrenDev.Application.Catalog.Pages.Entities;

public class PageDetailsDto : PageDto
{
    public string? Content { get; set; }
    public string? MediaUrl { get; set; }
    public ChapterDto Chapter { get; set; } = default!;
}
