using EvrenDev.Application.Features.PredefinedValues.Models;

namespace EvrenDev.Application.Features.PredefinedValues.Queries.GetAll;
public class GetAllPredefinedValuesQuery : IRequest<Result<PredefinedValuesVm>>
{
}

public class GetAllPredefinedValuesQueryHandler : IRequestHandler<GetAllPredefinedValuesQuery, Result<PredefinedValuesVm>>
{
    private readonly IStringLocalizer<GetAllPredefinedValuesQueryHandler> _localizer;
    public GetAllPredefinedValuesQueryHandler(IStringLocalizer<GetAllPredefinedValuesQueryHandler> localizer)
    {
        _localizer = localizer;
    }

    public async Task<Result<PredefinedValuesVm>> Handle(GetAllPredefinedValuesQuery request, CancellationToken cancellationToken)
    {
        var genders = Gender.SupportedGenders.Select(gender =>
            new PredefinedValueDto<string>()
            {
                Code = gender.Code,
                Name = _localizer[$"api.predefined-values.gender.{gender.Code}"]
            }
        ).ToList();

        var languages = Language.SupportedLanguages.Select(lang =>
            new PredefinedValueDto<string>()
            {
                Code = lang.Code,
                Name = _localizer[$"api.predefined-values.languages.{lang.Code}"]
            }
        ).ToList();

        var response = new PredefinedValuesVm()
        {
            Genders = genders,
            Languages = languages,
        };

        return await Task.FromResult(Result<PredefinedValuesVm>.Success(response));
    }
}
