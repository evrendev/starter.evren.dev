using Application.Common.Interfaces;
using EvrenDev.Application.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using static EvrenDev.Shared.Constants.Policies;

namespace EvrenDev.PublicApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly ICloudflareImageService _cloudflareImageService;
    private readonly ICloudflareR2Service _cloudflareR2Service;
    private readonly IStringLocalizer<FileController> _localizer;

    public FileController(
        ICloudflareImageService cloudflareImageService,
        ICloudflareR2Service cloudflareR2Service,
        IStringLocalizer<FileController> localizer)
    {
        _cloudflareImageService = cloudflareImageService;
        _cloudflareR2Service = cloudflareR2Service;
        _localizer = localizer;
    }

    [HttpPost("upload/image")]
    [Authorize(Policy = $"{Modules.Images}.{Permissions.Create}")]
    public async Task<ActionResult<string>> UploadImage(IFormFile file)
    {
        try
        {
            var imageId = await _cloudflareImageService.UploadImageAsync(file);
            return Ok(imageId);
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

    [HttpPost("upload/file")]
    [Authorize(Policy = $"{Modules.Files}.{Permissions.Create}")]
    public async Task<ActionResult<string>> UploadFile(IFormFile file, [FromQuery] string bucketName, [FromQuery] string? path = null)
    {
        try
        {
            var fileUrl = await _cloudflareR2Service.UploadFileAsync(file, bucketName, path);
            return Ok(fileUrl);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = true,
                message = ex.Message
            });
        }
    }

    [HttpDelete("file")]
    [Authorize(Policy = $"{Modules.Files}.{Permissions.Delete}")]
    public async Task<ActionResult> DeleteFile([FromQuery] string bucketName, [FromQuery] string key)
    {
        try
        {
            await _cloudflareR2Service.DeleteFileAsync(bucketName, key);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = true,
                message = ex.Message
            });
        }
    }
}
