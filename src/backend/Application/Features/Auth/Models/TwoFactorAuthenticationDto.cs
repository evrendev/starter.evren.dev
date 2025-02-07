using System;

namespace EvrenDev.Application.Features.Auth.Models;

public class TwoFactorAuthenticationDto
{
    public string SharedKey { get; set; } = string.Empty;
    public string QrCodeUri { get; set; } = string.Empty;
}

public class EnableTwoFactorAuthenticationRequest
{
    public string Code { get; set; } = string.Empty;
}

public class TwoFactorLoginRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool RememberMachine { get; set; }
}
