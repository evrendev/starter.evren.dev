using EvrenDev.Application.Catalog.Notes.Entities;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Notes.Specifications;

public class NotesByLessonPageSpec : Specification<Note, NoteDto>
{
    public NotesByLessonPageSpec(Guid lessonPageId) =>
        Query
            .Where(p => p.LessonPageId == lessonPageId)
            // CreatedOn is get-only on AuditableEntity and not mapped to a column;
            // UUIDv7 ids are time-ordered, so Id gives chronological order.
            .OrderBy(p => p.Id);
}
