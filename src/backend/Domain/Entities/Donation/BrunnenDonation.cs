using System.ComponentModel.DataAnnotations.Schema;

namespace EvrenDev.Domain.Entities.Donation;

[Table("Brunnen_Donations", Schema = "dbo")]
public class BrunnenDonation
{
    public Guid Id { get; set; }
    public string? Contact { get; set; }
    public string? Phone { get; set; }
    public string? Project { get; set; }
    public string? Banner { get; set; }
    public DateTime Date { get; set; }
    public int? ProjectNumber { get; set; }
    public string? ProjectCode { get; set; }
    public string? TransactionId { get; set; }
    public string? Source { get; set; }
    public string? Team { get; set; }
}
