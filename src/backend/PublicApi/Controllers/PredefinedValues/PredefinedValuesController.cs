using EvrenDev.Domain.Common.Enums;

namespace EvrenDev.PublicApi.Controllers.PredefinedValues;

public class PredefinedValuesController : VersionNeutralApiController
{
    [HttpGet]
    [AllowAnonymous]
    [OpenApiOperation("Get predefined values.", "")]
    public ApiResponse<object> Get()
    {
        var genders = GenderExtensions.ToList();
        var languages = LanguageExtensions.ToList();
        var response = new
        {
            Genders = genders,
            Languages = languages
        };

        return ApiResponse<object>.Success(response);
    }
}

