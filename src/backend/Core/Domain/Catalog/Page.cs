namespace EvrenDev.Domain.Catalog;

public enum PageContentType
{
    Text = 0,
    Video = 1,
    Image = 2,
    Quiz = 3,
    Embed = 4
}

public class Page : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = default!;
    public string Content { get; private set; } = default!;
    public PageContentType ContentType { get; private set; }
    public int Order { get; private set; }
    public string? MediaUrl { get; private set; }
    public Guid ChapterId { get; private set; }
    // Set by the import handler for machine-generated pages awaiting author
    // review; defaults false so the existing manual-creation flow is unaffected
    public bool NeedsReview { get; private set; }
    // True when Content came from the client-side pptx-to-html rich render (positioned
    // HTML) rather than the backend's own plain-text OpenXml extraction — the admin
    // editor and player both render this content in a sandboxed iframe instead of
    // Quill/v-html, since it carries its own inline layout/styles (see PPTX import Task F)
    public bool IsImported { get; private set; }
    public virtual Chapter Chapter { get; } = default!;
    public virtual ICollection<Note> Notes { get; private set; } = [];
    public virtual ICollection<PageProgress> Progress { get; private set; } = [];
    // Structural Quiz data (ContentType == Quiz) — managed entirely through this
    // aggregate, see ReplaceQuestions. Pages created before this model existed
    // simply have an empty collection; their Content keeps the legacy "(richtig)"
    // pattern the frontend still falls back to parsing.
    public virtual ICollection<Question> Questions { get; private set; } = [];

    public Page(string title, string content, PageContentType contentType,
        int order, Guid chapterId, string? mediaUrl = null, bool needsReview = false, bool isImported = false)
    {
        Title = title;
        Content = content;
        ContentType = contentType;
        Order = order;
        ChapterId = chapterId;
        MediaUrl = mediaUrl;
        NeedsReview = needsReview;
        IsImported = isImported;
    }

    public Page Update(string? title, string? content, PageContentType? contentType,
        int? order, string? mediaUrl, bool? needsReview = null, bool? isImported = null)
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

        if (isImported.HasValue && IsImported != isImported.Value)
            IsImported = isImported.Value;

        return this;
    }

    public Page Reorder(int newOrder)
    {
        Order = newOrder;
        return this;
    }

    // Replace-all: clears the existing Questions/Options and rebuilds them from
    // scratch. Called only when the admin form actually sent a Questions list
    // (see UpdatePageRequestHandler) — an empty list here deletes every question,
    // a null list at the caller means "don't touch Questions" and never reaches
    // this method at all.
    public Page ReplaceQuestions(IEnumerable<QuestionData> questions)
    {
        Questions.Clear();

        foreach (var questionData in questions)
        {
            var question = new Question(questionData.Prompt, questionData.Order, Id);

            foreach (var optionData in questionData.Options)
                question.Options.Add(new Option(optionData.Label, optionData.IsCorrect, optionData.Order, question.Id));

            Questions.Add(question);
        }

        return this;
    }
}
