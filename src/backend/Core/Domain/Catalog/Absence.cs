namespace EvrenDev.Domain.Catalog;

public class Absence : AuditableEntity, IAggregateRoot
{
    public Absence(DateTime startDate, DateTime endDate, string location, string employee, string calendarId,
        string? description = null)
    {
        StartDate = startDate;
        EndDate = endDate;
        Description = description;
        Location = location;
        Employee = employee;
        CalendarId = calendarId;
    }

    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public string? Description { get; private set; }
    public string Location { get; private set; } = default!;
    public string Employee { get; private set; } = default!;
    public string CalendarId { get; private set; } = default!;

    public Absence Update(DateTime startDate, DateTime endDate, string? description, string? location, string? employee,
        string? calendarId)
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
