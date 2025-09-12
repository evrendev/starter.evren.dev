namespace EvrenDev.Application.Identity.TwoFactorAuthentication;

public class TwoFactorAuthenticationDto
{
    public string SharedKey { get; set; } = string.Empty;
    public string QrCodeUri { get; set; } = string.Empty;
}

public class EnableTwoFactorAuthenticationRequest
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class VerifyTwoFactorAuthenticationRequest
{
    public string? Email { get; set; }
    public string Code { get; set; } = string.Empty;
}

public class DisableTwoFactorAuthenticationRequest
{
    public string Id { get; set; } = string.Empty;
}

public class TwoFactorLoginRequest
{
    public string UserId { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public bool RememberMachine { get; set; }
}
public class TwoFactorSetupRequest
{
    public string Id { get; set; } = string.Empty;
}
