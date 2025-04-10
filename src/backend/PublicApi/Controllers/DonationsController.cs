using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Models;
using EvrenDev.Application.Features.Donations.Fountain.Models;
using EvrenDev.Application.Features.Donations.Fountain.Queries.GetFountainDonationById;
using EvrenDev.Application.Features.Donations.Fountain.Queries.GetFountainDonations;
using EvrenDev.Application.Features.Donations.Fountain.Commands.CreateFountainDonation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using static EvrenDev.Shared.Constants.Policies;
using EvrenDev.Application.Features.Donations.Fountain.Commands.ChangeMediaStatus;

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
    public async Task<ActionResult<PaginatedList<BasicFountainDonationDto>>> GetAll([FromQuery] GetFountainDonationsQuery query)
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
    public async Task<ActionResult<Guid>> Create(CreateFountainDonationCommand command)
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

    [HttpPut("fountain/media-status/{id}")]
    [Authorize(Policy = $"{Modules.Donations}.{Permissions.Edit}")]
    public async Task<ActionResult<Guid>> Create(Guid id, ChangeMediaStatusCommand command)
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
}
