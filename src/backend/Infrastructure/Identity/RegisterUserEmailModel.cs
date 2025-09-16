namespace EvrenDev.Infrastructure.Identity;

public class RegisterUserEmailModel
{
    public string FullName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Url { get; set; } = default!;
}
