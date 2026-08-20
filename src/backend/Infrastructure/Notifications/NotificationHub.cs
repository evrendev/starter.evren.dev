using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Interfaces;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TenantInfo = EvrenDev.Domain.Multitenancy.TenantInfo;

namespace EvrenDev.Infrastructure.Notifications;

// ITenantInfo is no longer directly DI-resolvable as of Finbuckle v10 (Task S3).
[Authorize]
public class NotificationHub(IMultiTenantContextAccessor<TenantInfo> multiTenantContextAccessor, ILogger<NotificationHub> logger)
    : Hub, ITransientService
{
    private string? CurrentTenantId => multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id;

    public override async Task OnConnectedAsync()
    {
        if (CurrentTenantId is null)
            throw new UnauthorizedException("Authentication Failed.");

        await Groups.AddToGroupAsync(Context.ConnectionId, $"GroupTenant-{CurrentTenantId}");

        await base.OnConnectedAsync();

        logger.LogInformation("A client connected to NotificationHub: {connectionId}", Context.ConnectionId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"GroupTenant-{CurrentTenantId}");

        await base.OnDisconnectedAsync(exception);

        logger.LogInformation("A client disconnected from NotificationHub: {connectionId}", Context.ConnectionId);
    }
}
