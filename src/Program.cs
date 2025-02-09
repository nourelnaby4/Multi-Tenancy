using API.Data;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ITenantService, TenantService>();

builder.Services.Configure<TenantSettings>(builder.Configuration.GetSection(nameof(TenantSettings)));

TenantSettings options = new();
builder.Configuration.GetSection(nameof(TenantSettings)).Bind(options);

var defaultDbProvider = options.Default.DBProvider;

if (defaultDbProvider.ToLower() == "mssql")
{
    builder.Services.AddDbContext<ApplicationDbContext>(m => m.UseSqlServer());
}

foreach(var tenant in options.Tenants)
{
    var connectionString = tenant.ConnectionString ?? options.Default.ConnectionString;

    using var scope = builder.Services.BuildServiceProvider().CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.SetConnectionString(connectionString);

    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }
}
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
