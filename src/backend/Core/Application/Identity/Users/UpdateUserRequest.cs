namespace EvrenDev.Application.Identity.Users;

public class UpdateUserRequest
{
    public Gender? Gender { get; set; }
    public Language? Language { get; set; }
    public string Id { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public DateTime? Birthday { get; set; }
    public string? PlaceOfBirth { get; set; }
}
