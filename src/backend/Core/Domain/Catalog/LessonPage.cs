namespace EvrenDev.Domain.Catalog;

public enum LessonPageContentType
{
    Text = 0,
    Video = 1,
    Image = 2,
    Quiz = 3,
    Embed = 4
}

public class LessonPage : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = default!;
    public string Content { get; private set; } = default!;
    public LessonPageContentType ContentType { get; private set; }
    public int Order { get; private set; }
    public string? MediaUrl { get; private set; }
    public Guid LessonId { get; private set; }
    // Set by the import handler for machine-generated pages awaiting author
    // review; defaults false so the existing manual-creation flow is unaffected
    public bool NeedsReview { get; private set; }
    public virtual Lesson Lesson { get; } = default!;
    public virtual ICollection<Note> Notes { get; private set; } = [];
    public virtual ICollection<LessonPageProgress> Progress { get; private set; } = [];

    public LessonPage(string title, string content, LessonPageContentType contentType,
        int order, Guid lessonId, string? mediaUrl = null, bool needsReview = false)
    {
        Title = title;
        Content = content;
        ContentType = contentType;
        Order = order;
        LessonId = lessonId;
        MediaUrl = mediaUrl;
        NeedsReview = needsReview;
    }

    public LessonPage Update(string? title, string? content, LessonPageContentType? contentType,
        int? order, string? mediaUrl, bool? needsReview = null)
    {
        if (title is not null && !Title.Equals(title))
            Title = title;

        if (content is not null && !Content.Equals(content))
            Content = content;

        if (contentType.HasValue && ContentType != contentType.Value)
            ContentType = contentType.Value;

        if (order.HasValue && Order != order.Value)
            Order = order.Value;

        if (mediaUrl is not null && !string.Equals(MediaUrl, mediaUrl))
            MediaUrl = mediaUrl;

        if (needsReview.HasValue && NeedsReview != needsReview.Value)
            NeedsReview = needsReview.Value;

        return this;
    }

    public LessonPage Reorder(int newOrder)
    {
        Order = newOrder;
        return this;
    }
}
