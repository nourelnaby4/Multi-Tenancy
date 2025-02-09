using API.Settings;

namespace API.Services;

public class TenantService : ITenantService
{
    private Tenant? _currentTenant;
    private HttpContext? _httpContext;
    private readonly TenantSettings _tenantSettings;

    public TenantService(IHttpContextAccessor contextAccessor,TenantSettings tenantSettings)
    {
        _httpContext = contextAccessor.HttpContext;
        _tenantSettings = tenantSettings;

        if (_httpContext is not null)
        {
            if (_httpContext.Request.Headers.TryGetValue("tenant", out var tenantId))
            {
                SetCurrentTenant(tenantId!);
            }
            else
            {
                throw new Exception("No tenant provided!");
            }
        }
    }

    public string? GetConnectionString()
    {
        var currentConnectionString =
            _currentTenant is null 
            ? _tenantSettings.Default.ConnectionString : _currentTenant.ConnectionString;

        return currentConnectionString;
    }

    public Tenant? GetCurrentTenant()
    {
        return _currentTenant;
    }

    public string? GetDatabaseProvider()
    {
        return _tenantSettings.Default.DBProvider;
    }

    private void SetCurrentTenant(string tenantId)
    {
        _currentTenant = _tenantSettings.Tenants.FirstOrDefault(x => x.TenantId == tenantId);
        if (_currentTenant is null)
        {
            throw new Exception("Invalid Tenant ID!!");
        }

        if (string.IsNullOrEmpty(_currentTenant.ConnectionString))
        {
            _currentTenant.ConnectionString = _tenantSettings.Default.ConnectionString;
        }
    }
}
