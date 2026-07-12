namespace EvrenDev.Application.Catalog.Notes.Entities;

public class NoteDto : IDto
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public Guid LessonPageId { get; set; }
    public string? Content { get; set; }
}
