using EvrenDev.Shared.Multitenancy;
using Finbuckle.MultiTenant.Abstractions;

namespace EvrenDev.Domain.Multitenancy;

public class TenantInfo : ITenantInfo
{
    public TenantInfo()
    {
    }

    public TenantInfo(string id, string name, string? connectionString, string adminEmail, string? issuer = null,
        bool isActive = true, DateTime? validUpto = null)
    {
        Id = id;
        Identifier = id;
        Name = name;
        ConnectionString = connectionString ?? string.Empty;
        AdminEmail = adminEmail;
        IsActive = isActive;
        Issuer = issuer;

        // Add Default 1 Month Validity for all new tenants. Something like a DEMO period for tenants.
        ValidUpto = validUpto ?? DateTime.UtcNow.AddMonths(1);
    }

    /// <summary>
    ///     The actual TenantId, which is also used in the TenantId shadow property on the multitenant entities.
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    ///     The identifier that is used in headers/routes/querystrings. This is set to the same as Id to avoid confusion.
    /// </summary>
    public string Identifier { get; set; } = default!;

    public string Name { get; set; } = default!;
    public string ConnectionString { get; set; } = default!;

    public string AdminEmail { get; } = default!;
    public bool IsActive { get; private set; }
    public DateTime ValidUpto { get; private set; }

    /// <summary>
    ///     Used by AzureAd Authorization to store the AzureAd Tenant Issuer to map against.
    /// </summary>
    public string? Issuer { get; set; }

    // v10's ITenantInfo only declares get-only Id/Identifier (Name and ConnectionString
    // were dropped from the interface in v7/v10 respectively, and the setters that used
    // to be required by the old interface are gone too) — Name and ConnectionString stay
    // as plain public get/set properties above, just no longer part of the interface
    // contract. See Task S3 migration. Both getters are non-nullable on the interface
    // (Task Y1) — Id/Identifier are themselves never null after construction (`= default!`
    // only guards the parameterless ctor path, immediately overwritten by the real ctor).
    string ITenantInfo.Id => Id;

    string ITenantInfo.Identifier => Identifier;

    public void AddValidity(int months)
    {
        ValidUpto = ValidUpto.AddMonths(months);
    }

    public void SetValidity(in DateTime validTill)
    {
        ValidUpto = ValidUpto < validTill
            ? validTill
            : throw new Exception("Subscription cannot be backdated.");
    }

    public void Activate()
    {
        if (Id == MultitenancyConstants.Root.Id)
            throw new InvalidOperationException("Invalid Tenant");

        IsActive = true;
    }

    public void Deactivate()
    {
        if (Id == MultitenancyConstants.Root.Id)
            throw new InvalidOperationException("Invalid Tenant");

        IsActive = false;
    }
}
