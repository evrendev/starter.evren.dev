using EvrenDev.Application.Catalog.Notes.Entities;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Notes.Specifications;

public class NoteByIdSpec : Specification<Note, NoteDto>, ISingleResultSpecification<Note>
{
    public NoteByIdSpec(Guid id) =>
        Query
            .Where(p => p.Id == id);
}
