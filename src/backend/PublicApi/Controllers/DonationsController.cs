using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Models;
using EvrenDev.Application.Features.Donations.Fontain.Models;
using EvrenDev.Application.Features.Donations.Fontain.Queries.GetFontainDonationById;
using EvrenDev.Application.Features.Donations.Fontain.Queries.GetFontainDonations;
using EvrenDev.Application.Features.Donations.Fontain.Commands.CreateFontainDonation;
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

    [HttpGet("fontain")]
    [Authorize(Policy = $"{Modules.Donations}.{Permissions.Read}")]
    public async Task<ActionResult<PaginatedList<BasicFontainDonationDto>>> GetAll([FromQuery] GetFontainDonationsQuery query)
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

    [HttpGet("fontain/{id}")]
    [Authorize(Policy = $"{Modules.Donations}.{Permissions.Read}")]
    public async Task<ActionResult<FullFontainDonationDto>> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetFontainDonationByIdQuery { Id = id });
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

    [HttpPost("fontain")]
    [Authorize(Policy = $"{Modules.Donations}.{Permissions.Create}")]
    public async Task<ActionResult<Guid>> Create(CreateFontainDonationCommand command)
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
