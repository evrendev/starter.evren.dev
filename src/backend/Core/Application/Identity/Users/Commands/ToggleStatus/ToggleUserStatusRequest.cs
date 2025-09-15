namespace EvrenDev.Application.Identity.Users.Commands.ToggleStatus;

public class ToggleUserStatusRequest
{
    public bool ActivateUser { get; set; }
    public string? UserId { get; set; }
}
