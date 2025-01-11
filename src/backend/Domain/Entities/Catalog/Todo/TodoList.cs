using EvrenDev.Domain.Interfaces;

namespace EvrenDev.Domain.Entities.Catalog;

[AuditInclude]
public class TodoList : BaseAuditableEntity, ITenant
{
    public string? Title { get; set; }

    public Colour Colour { get; set; } = Colour.White;

    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();
}
