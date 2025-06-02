using EvrenDev.Infrastructure.Services.Model;

namespace EvrenDev.Application.Common.Interfaces;
public interface ISendmailService
{
    Task<bool> SendEmailAsync(AhasendRequest? request);
}
