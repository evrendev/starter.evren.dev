namespace EvrenDev.Domain.Catalog;

// Input shape for Page.ReplaceQuestions — not persisted entities, just the data
// needed to (re)build the Questions/Options collection in one call.
public record OptionData(string Label, bool IsCorrect, int Order);

public record QuestionData(string Prompt, int Order, IEnumerable<OptionData> Options);

public class Question : AuditableEntity
{
    public string Prompt { get; private set; } = default!;
    public int Order { get; private set; }
    public Guid PageId { get; private set; }
    public virtual Page Page { get; } = default!;
    public virtual ICollection<Option> Options { get; private set; } = [];

    public Question(string prompt, int order, Guid pageId)
    {
        Prompt = prompt;
        Order = order;
        PageId = pageId;
    }

    public Question Update(string? prompt, int? order)
    {
        if (prompt is not null && !Prompt.Equals(prompt))
            Prompt = prompt;

        if (order.HasValue && Order != order.Value)
            Order = order.Value;

        return this;
    }
}
