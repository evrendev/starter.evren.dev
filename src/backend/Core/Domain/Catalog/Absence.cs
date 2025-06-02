namespace EvrenDev.Domain.Catalog;

public class Absence : AuditableEntity, IAggregateRoot
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Employee { get; set; }
    public string? CalendarId { get; set; }

    public Absence Update(DateTime startDate, DateTime endDate, string? description, string? location, string? employee, string? calendarId)
    {
        if (StartDate != startDate)
            StartDate = startDate;
        if (EndDate != endDate)
            EndDate = endDate;
        if (description is not null && Description?.Equals(description) is not true)
            Description = description;
        if (location is not null && Location?.Equals(location) is not true)
            Location = location;
        if (employee is not null && Employee?.Equals(employee) is not true)
            Employee = employee;
        if (calendarId is not null && CalendarId?.Equals(calendarId) is not true)
            CalendarId = calendarId;

        return this;
    }
}
