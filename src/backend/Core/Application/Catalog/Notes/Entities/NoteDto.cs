namespace EvrenDev.Application.Catalog.Notes.Entities;

public class NoteDto : IDto
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public Guid LessonPageId { get; set; }
    public string? Content { get; set; }

    // Mapped from the entity's LastModifiedOn (set at create time), NOT from
    // AuditableEntity.CreatedOn — that property is get-only, has no DB column
    // and re-evaluates to "now" on every query. See docs/lms-domain.md.
    public DateTime? CreatedOn { get; set; }
}
