namespace EvrenDev.Domain.Catalog;

public class Option : AuditableEntity
{
    public string Label { get; private set; } = default!;
    public bool IsCorrect { get; private set; }
    public int Order { get; private set; }
    public Guid QuestionId { get; private set; }
    public virtual Question Question { get; } = default!;

    public Option(string label, bool isCorrect, int order, Guid questionId)
    {
        Label = label;
        IsCorrect = isCorrect;
        Order = order;
        QuestionId = questionId;
    }

    public Option Update(string? label, bool? isCorrect, int? order)
    {
        if (label is not null && !Label.Equals(label))
            Label = label;

        if (isCorrect.HasValue && IsCorrect != isCorrect.Value)
            IsCorrect = isCorrect.Value;

        if (order.HasValue && Order != order.Value)
            Order = order.Value;

        return this;
    }
}
