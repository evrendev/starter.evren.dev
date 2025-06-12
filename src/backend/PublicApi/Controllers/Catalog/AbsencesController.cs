using EvrenDev.Application.Catalog.Absences.Commands.CreateAbsence;
using EvrenDev.Application.Catalog.Absences.Commands.DeleteAbsence;
using EvrenDev.Application.Catalog.Absences.Commands.UpdateAbsence;
using EvrenDev.Application.Catalog.Absences.Entities;
using EvrenDev.Application.Catalog.Absences.Queries.Get;
using EvrenDev.Application.Catalog.Absences.Queries.Search;

namespace EvrenDev.PublicApi.Controllers.Catalog;

public class AbsencesController : VersionedApiController
{
    [HttpPost("search")]
    [MustHavePermission(ApiAction.View, ApiResource.Absences)]
    [OpenApiOperation("Get all absences.", "")]
    public Task<PaginationResponse<AbsenceDto>> SearchAsync(SearchAbsencesRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [MustHavePermission(ApiAction.View, ApiResource.Absences)]
    [OpenApiOperation("Get absence details.", "")]
    public async Task<ApiResponse<AbsenceDto>> GetAsync(Guid id)
    {
        var data = await Mediator.Send(new GetAbsencesQuery(id));
        if (data == null)
            throw new NotFoundException($"Absence with ID '{id}' not found.");

        return ApiResponse<AbsenceDto>.Success(data);
    }

    [HttpPost]
    [MustHavePermission(ApiAction.Create, ApiResource.Absences)]
    [OpenApiOperation("Create a new absence.", "")]
    public Task<Guid> CreateAsync(CreateAbsenceCommand request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(ApiAction.Update, ApiResource.Absences)]
    [OpenApiOperation("Update a absence.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateAbsenceCommand command, Guid id)
    {
        return id != command.Id
            ? BadRequest()
            : Ok(await Mediator.Send(command));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(ApiAction.Delete, ApiResource.Absences)]
    [OpenApiOperation("Delete a absence.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteAbsenceCommand(id));
    }
}
