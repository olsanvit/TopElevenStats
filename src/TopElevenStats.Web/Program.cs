using BlazorTopEleven.Components;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;
using Serilog.Exceptions;
using Services;
using SharedServices;
using SharedServices.Services;
using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.SessionStorage;
using ApexCharts;

var builder = WebApplication.CreateBuilder(args);

Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "Logs"));
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.WithMachineName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30, shared: true,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();
builder.Host.UseSerilog();

builder.Logging.AddDebug();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.Configure<CircuitOptions>(o =>
{
    o.DisconnectedCircuitRetentionPeriod = TimeSpan.FromHours(8);
    o.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(2);
});
builder.Services.Configure<HubOptions>(o =>
{
    o.KeepAliveInterval = TimeSpan.FromSeconds(15);
    o.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
    o.HandshakeTimeout = TimeSpan.FromSeconds(30);
});

var cs = "";
#if DEBUG
cs = builder.Configuration.GetConnectionString("DefaultConnection1QNAP");
#else
cs = builder.Configuration.GetConnectionString("DefaultConnection1");
#endif
var dsb = new NpgsqlDataSourceBuilder(cs);
dsb.EnableDynamicJson();
var dataSource = dsb.Build();

builder.Services.AddDbContextFactory<AppDbContextTopEleven>(options =>
    options.UseNpgsql(dataSource, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly("SharedServices");
        npgsqlOptions.CommandTimeout(180);
        npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null);
    })
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors());

builder.Services.AddScoped<AppDbContextTopEleven>(sp =>
    sp.GetRequiredService<IDbContextFactory<AppDbContextTopEleven>>().CreateDbContext());

builder.Services.AddHttpContextAccessor();
builder.Services.AddCors();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<AlertService>();
builder.Services.AddBlazoredModal();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddApexCharts();
builder.Services.AddScoped<ErrorService<AppDbContextTopEleven>>();
builder.Services.AddScoped<EfCoreService<AppDbContextTopEleven>>();
builder.Services.AddScoped(provider =>
    new MigrationService<AppDbContextTopEleven>(
        provider,
        provider.GetRequiredService<ILogger<MigrationService<AppDbContextTopEleven>>>(),
        "SharedServices",
        "BlazorTopEleven"
    ));
builder.Services.AddSingleton<ThemeService>(_ => new ThemeService(builder.Configuration));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.WebHost.ConfigureKestrel(k =>
{
    k.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(3);
    k.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
});

AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
    Log.Fatal(e.ExceptionObject as Exception, "UNHANDLED AppDomain exception");
TaskScheduler.UnobservedTaskException += (sender, e) =>
{
    Log.Fatal(e.Exception, "UNOBSERVED task exception");
    e.SetObserved();
};

var app = builder.Build();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

using (var scope = app.Services.CreateScope())
{
    var migrationService = scope.ServiceProvider.GetRequiredService<MigrationService<AppDbContextTopEleven>>();
    await migrationService.MigrateDatabaseAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

if (!app.Environment.IsProduction())
    app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseSession();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Lifetime.ApplicationStopping.Register(() => Log.Warning("Application stopping — flushing logs..."));

try { app.Run(); }
catch (Exception ex) { Log.Fatal(ex, "Host terminated unexpectedly"); }
finally { Log.CloseAndFlush(); }
