namespace API.Settings;

public class TenantSettings
{
    public Configuration Default { get; set; } = default!;
    public List<Tenant> Tenants { get; set; } = new();
}
