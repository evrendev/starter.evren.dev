namespace EvrenDev.Domain.Catalog;

public class Note : AuditableEntity, IAggregateRoot
{
    public string UserId { get; private set; } = default!;
    public Guid PageId { get; private set; } = default!;
    public string Content { get; private set; } = default!;
    public virtual Page? Page { get; } = default!;

    public Note(string userId, Guid pageId, string content)
    {
        UserId = userId;
        PageId = pageId;
        Content = content;
    }

    public Note Update(string? userId, Guid pageId, string? content)
    {
        if (userId is not null && !UserId.Equals(userId))
            UserId = userId;

        if (pageId != Guid.Empty && PageId != pageId)
            PageId = pageId;

        if (content is not null && !Content.Equals(content))
            Content = content;

        return this;
    }
}
