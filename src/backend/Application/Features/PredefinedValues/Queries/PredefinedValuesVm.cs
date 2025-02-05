namespace EvrenDev.Application.Features.PredefinedValues.Queries;

public class PredefinedValuesVm
{
    public IReadOnlyCollection<PredefinedValueDto<string>> Genders { get; init; } = Array.Empty<PredefinedValueDto<string>>();
    public IReadOnlyCollection<PredefinedValueDto<string>> Languages { get; init; } = Array.Empty<PredefinedValueDto<string>>();
}
