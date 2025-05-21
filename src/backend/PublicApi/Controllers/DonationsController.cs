using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Models;
using EvrenDev.Application.Features.Donations.Fountain.Commands.ChangeMediaInformation;
using EvrenDev.Application.Features.Donations.Fountain.Commands.ChangeTeamName;
using EvrenDev.Application.Features.Donations.Fountain.Commands.CreateAutomaticFountainDonation;
using EvrenDev.Application.Features.Donations.Fountain.Commands.CreateEmptyDonation;
using EvrenDev.Application.Features.Donations.Fountain.Commands.CreateFountainDonation;
using EvrenDev.Application.Features.Donations.Fountain.Commands.DeleteDonation;
using EvrenDev.Application.Features.Donations.Fountain.Commands.UpdateConstructionTeamNotified;
using EvrenDev.Application.Features.Donations.Fountain.Commands.UpdateDonorNotified;
using EvrenDev.Application.Features.Donations.Fountain.Commands.UpdateFountainDonation;
using EvrenDev.Application.Features.Donations.Fountain.Models;
using EvrenDev.Application.Features.Donations.Fountain.Queries.GetDonationsOverview;
using EvrenDev.Application.Features.Donations.Fountain.Queries.GetFountainDonationById;
using EvrenDev.Application.Features.Donations.Fountain.Queries.GetFountainDonations;
using EvrenDev.PublicApi.Hub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHubContext<NotificationHub> _hubContext;

    public DonationsController(IMediator mediator,
        IHttpClientFactory httpClientFactory,
        IHubContext<NotificationHub> hubContext,
        IStringLocalizer<DonationsController> localizer)
    {
        _httpClientFactory = httpClientFactory;
        _hubContext = hubContext;
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

    [HttpPost("fountain/automatic-donation")]
    [AllowAnonymous]
    public async Task<ActionResult<BasicFountainDonationDto>> CreateAutomaticDonation(CreateAutomaticFountainDonationCommand command)
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

    [HttpPut("fountain/donor-notified/{id}")]
    [Authorize(Policy = $"{Modules.Donations}.{Permissions.Edit}")]
    public async Task<ActionResult<BasicFountainDonationDto>> DonorNotified(Guid id)
    {
        try
        {
            var command = new UpdateDonorNotifiedCommand { Id = id };
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

    [HttpPut("fountain/construction-team-notified/{id}")]
    [Authorize(Policy = $"{Modules.Donations}.{Permissions.Edit}")]
    public async Task<ActionResult<BasicFountainDonationDto>> ConstructionTeamNotified(Guid id)
    {
        try
        {
            var command = new UpdateConstructionTeamNotifiedCommand { Id = id };
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

    [HttpGet("fountain/overview")]
    [Authorize(Policy = $"{Modules.Donations}.{Permissions.Read}")]
    public async Task<ActionResult> GetCounts([FromQuery] GetDonationsOverviewQuery query)
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

    [HttpGet("fountain/get-last-donations")]
    [Authorize(Policy = $"{Modules.Donations}.{Permissions.Read}")]
    public async Task<ActionResult> GetLastDonations()
    {
        try
        {
            var url = "https://prod-29.germanywestcentral.logic.azure.com:443/workflows/bc6612ba523e487cb94d202b736865a0/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=m19ivN7YS_IXoEq1LZBQLuKKwH5AWbcMcefQVutXWuM";

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return BadRequest(new { message = "Failed to check donations." });

            var content = await response.Content.ReadAsStringAsync();
            return Ok(new { message = content });
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

    [HttpGet("fountain/notifications")]
    [AllowAnonymous]
    public async Task Notifications([FromQuery] bool donationFound = false)
    {
        var message = donationFound
            ? _localizer["api.donations.fountain.notification.found"].Value
            : _localizer["api.donations.fountain.notification.not-found"].Value;

        await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
    }
}
