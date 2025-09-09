using EvrenDev.Domain.Common.Enums;

namespace EvrenDev.PublicApi.Controllers.PredefinedValues;

public class PredefinedValuesController : VersionNeutralApiController
{
    [HttpGet]
    [AllowAnonymous]
    [OpenApiOperation("Get predefined values.", "")]
    public IActionResult Get()
    {
        var genders = GenderExtensions.ToList();
        var languages = LanguageExtensions.ToList();
        var response = new
        {
            Genders = genders,
            Languages = languages
        };

        return Ok(response);
    }
}
