using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Models;
using EvrenDev.Application.Features.Donations.Fountain.Models;
using EvrenDev.Application.Features.Donations.Fountain.Queries.GetFountainDonationById;
using EvrenDev.Application.Features.Donations.Fountain.Queries.GetFountainDonations;
using EvrenDev.Application.Features.Donations.Fountain.Commands.CreateFountainDonation;
using EvrenDev.Application.Features.Donations.Fountain.Commands.ChangeMediaInformation;
using EvrenDev.Application.Features.Donations.Fountain.Commands.DeleteDonation;
using EvrenDev.Application.Features.Donations.Fountain.Commands.UpdateFountainDonation;
using EvrenDev.Application.Features.Donations.Fountain.Commands.CreateEmptyDonation;
using EvrenDev.Application.Features.Donations.Fountain.Commands.ChangeTeamName;
using EvrenDev.Application.Features.Donations.Fountain.Queries.GetCounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using static EvrenDev.Shared.Constants.Policies;

namespace EvrenDev.PublicApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DonationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<DonationsController> _localizer;

    public DonationsController(IMediator mediator,
        IStringLocalizer<DonationsController> localizer)
    {
        _mediator = mediator;
        _localizer = localizer;
    }

    [HttpGet("fountain")]
    [Authorize(Policy = $"{Modules.Donations}.{Permissions.Read}")]
    public async Task<ActionResult<PaginatedList<BasicFountainDonationDto>>> GetAllFountainDonations([FromQuery] GetFountainDonationsQuery query)
    {
        try
        {
            var result = await _mediator.Send(query);
            return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                Error = true,
                message = _localizer["api.validations.failed"].Value,
                Errors = ex.Errors.Select(x => new
                {
                    key = x.Key.ToLowerInvariant(),
                    value = x.Value[0]
                }).ToList()
            });
        }
    }

    [HttpGet("fountain/{id}")]
    [Authorize(Policy = $"{Modules.Donations}.{Permissions.Read}")]
    public async Task<ActionResult<FullFountainDonationDto>> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetFountainDonationByIdQuery { Id = id });
            return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                Error = true,
                message = _localizer["api.validations.failed"].Value,
                Errors = ex.Errors.Select(x => new
                {
                    key = x.Key.ToLowerInvariant(),
                    value = x.Value[0]
                }).ToList()
            });
        }
    }

    [HttpPost("fountain")]
    [Authorize(Policy = $"{Modules.Donations}.{Permissions.Create}")]
    public async Task<ActionResult<Guid>> CreateFountainDonation(CreateFountainDonationCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                Error = true,
                message = _localizer["api.validations.failed"].Value,
                Errors = ex.Errors.Select(x => new
                {
                    key = x.Key.ToLowerInvariant(),
                    value = x.Value[0]
                }).ToList()
            });
        }
    }

    [HttpPost("fountain/empty-donation")]
    [Authorize(Policy = $"{Modules.Donations}.{Permissions.Create}")]
    public async Task<ActionResult<BasicFountainDonationDto>> CreateEmptyDonation(CreateEmptyDonationCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                Error = true,
                message = _localizer["api.validations.failed"].Value,
                Errors = ex.Errors.Select(x => new
                {
                    key = x.Key.ToLowerInvariant(),
                    value = x.Value[0]
                }).ToList()
            });
        }
    }

    [HttpPut("fountain/media-information/{id}")]
    [Authorize(Policy = $"{Modules.Donations}.{Permissions.Edit}")]
    public async Task<ActionResult<Guid>> ChangeMediaInformation(Guid id, ChangeMediaInformationCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                Error = true,
                message = _localizer["api.validations.failed"].Value,
                Errors = ex.Errors.Select(x => new
                {
                    key = x.Key.ToLowerInvariant(),
                    value = x.Value[0]
                }).ToList()
            });
        }
    }

    [HttpPut("fountain/team-name/{id}")]
    [Authorize(Policy = $"{Modules.Donations}.{Permissions.Edit}")]
    public async Task<ActionResult<Guid>> ChangeTeamName(Guid id, ChangeTeamNameCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                Error = true,
                message = _localizer["api.validations.failed"].Value,
                Errors = ex.Errors.Select(x => new
                {
                    key = x.Key.ToLowerInvariant(),
                    value = x.Value[0]
                }).ToList()
            });
        }
    }

    [HttpDelete("fountain/{id}")]
    [Authorize(Policy = $"{Modules.Donations}.{Permissions.Delete}")]
    public async Task<ActionResult<bool>> DeleteFountainDonation(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteDonationCommand { Id = id });
            return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                Error = true,
                message = _localizer["api.validations.failed"].Value,
                Errors = ex.Errors.Select(x => new
                {
                    key = x.Key.ToLowerInvariant(),
                    value = x.Value[0]
                }).ToList()
            });
        }
    }
    [HttpPut("fountain/{id}")]
    [Authorize(Policy = $"{Modules.Donations}.{Permissions.Delete}")]
    public async Task<ActionResult<bool>> UpdateFountainDonation(Guid id, UpdateFountainDonationCommand command)
    {
        try
        {
            if (id != command.Id)
                return BadRequest();

            var result = await _mediator.Send(command);
            return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                Error = true,
                message = _localizer["api.validations.failed"].Value,
                Errors = ex.Errors.Select(x => new
                {
                    key = x.Key.ToLowerInvariant(),
                    value = x.Value[0]
                }).ToList()
            });
        }
    }

    [HttpGet("fountain/counts")]
    [Authorize(Policy = $"{Modules.Donations}.{Permissions.Read}")]
    public async Task<ActionResult> GetCounts([FromQuery] GetCountsQuery query)
    {
        try
        {
            var result = await _mediator.Send(query);
            return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                Error = true,
                message = _localizer["api.validations.failed"].Value,
                Errors = ex.Errors.Select(x => new
                {
                    key = x.Key.ToLowerInvariant(),
                    value = x.Value[0]
                }).ToList()
            });
        }
    }

}
