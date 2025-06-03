namespace EvrenDev.Application.Identity.Tokens;

public record TokenRequest(string Email, string Password, string Response, bool RememberMe);
