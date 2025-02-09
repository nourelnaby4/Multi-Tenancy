namespace API.Data;

public class ApplicationDbContext : DbContext
{
    public string TenantId { get; set; }
    private readonly ITenantService _tenantService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantService) : base(options)
    {
        _tenantService = tenantService;
        TenantId = tenantService.GetCurrentTenant()?.TenantId;
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var tenantConnectionString = _tenantService.GetConnectionString();

        if (!string.IsNullOrEmpty(tenantConnectionString))
        { 
            var dbProvider = _tenantService.GetDatabaseProvider();

            if(dbProvider.ToLower() == "mssql")
            {
                optionsBuilder.UseSqlServer(tenantConnectionString);
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasQueryFilter(e=>e.TenantId == TenantId);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach(var entry in ChangeTracker.Entries<IMustHaveTenant>().Where(e=>e.State == EntityState.Added))
        {
            entry.Entity.TenantId = TenantId;
        }

        return base.SaveChangesAsync();
    }
}
