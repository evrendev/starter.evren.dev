namespace EvrenDev.Application.Catalog.Pages.Entities;

public class OptionDto : IDto
{
    public string Label { get; set; } = default!;
    public bool IsCorrect { get; set; }
    public int Order { get; set; }
}

public class QuestionDto : IDto
{
    public string Prompt { get; set; } = default!;
    public int Order { get; set; }
    public List<OptionDto> Options { get; set; } = [];
}
