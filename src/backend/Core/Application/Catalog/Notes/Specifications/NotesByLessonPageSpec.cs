using EvrenDev.Application.Catalog.Notes.Entities;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Notes.Specifications;

public class NotesByLessonPageSpec : Specification<Note, NoteDto>
{
    public NotesByLessonPageSpec(Guid lessonPageId) =>
        Query
            .Where(p => p.LessonPageId == lessonPageId)
            .OrderBy(p => p.CreatedOn);
}
