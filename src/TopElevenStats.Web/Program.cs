using BlazorTopEleven.Components;
using BlazorTopEleven.Web.Validators;
using FluentValidation;
using SharedServices.Models.TopEleven;
using MercenariesAndBeasts.Infrastructure;
using MercenariesAndBeasts.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Npgsql;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using Services;
using SharedServices;
using SharedServices.Services;
using SharedServices.Models.Achievement;
using TopElevenStats.Web.Achievements;
using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.SessionStorage;
using ApexCharts;
using System.Security.Claims;

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
    .WriteTo.PostgreSQL(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection1") ?? "",
        tableName: "Logs",
        columnOptions: (IDictionary<string, ColumnWriterBase>?)null,
        needAutoCreateTable: true,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Logging.AddDebug();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddControllers();

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

// AddMabDbContext = AddDbContextFactory + scoped AddDbContext (Identity potřebuje scoped)
builder.Services.AddMabDbContext<AppDbContextGames>(dataSource);

// Identity + Google OAuth
builder.Services.AddMabAuth<AppDbContextGames>(builder.Configuration);

// Identity UI vyžaduje IEmailSender — no-op implementace
builder.Services.AddSingleton<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender,
    NoOpEmailSender>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddCors();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<AchievementService>(sp =>
    new AchievementService(
        sp.GetRequiredService<ToastService>(),
        sp.GetRequiredService<IWebHostEnvironment>())
    {
        Definitions = TopElevenAchievements.All
    });
builder.Services.AddScoped<AlertService>();
builder.Services.AddBlazoredModal();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddApexCharts();
builder.Services.AddScoped<ErrorService<AppDbContextGames>>();
builder.Services.AddScoped<EfCoreService<AppDbContextGames>>();
builder.Services.AddSingleton<ThemeService>(_ => new ThemeService(builder.Configuration));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHealthChecks();
builder.Services.AddScoped<IValidator<TopElevenPlayer>, TopElevenPlayerValidator>();
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

if (string.IsNullOrWhiteSpace(builder.Configuration["Authentication:Google:ClientId"]))
    Log.Warning("Google OAuth ClientId is not configured — Google login will not work");

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

var pathBase = builder.Configuration["PathBase"];
if (!string.IsNullOrWhiteSpace(pathBase))
    app.UsePathBase(pathBase);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

if (!app.Environment.IsProduction())
    app.UseHttpsRedirection();

app.MapStaticAssets();
app.UseStaticFiles();
app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseAntiforgery();

app.MapRazorPages();
app.MapHealthChecks("/health");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(MercenariesAndBeasts.Infrastructure.Components.Account.Login).Assembly);

app.MapControllers();

// ── Google OAuth external login endpoints ──────────────────────────────────
app.MapPost("/Identity/Account/ExternalLogin", async (
    HttpContext http,
    SignInManager<AppUser> signInManager) =>
{
    var provider  = http.Request.Form["provider"].ToString();
    var returnUrl = http.Request.Form["returnUrl"].ToString() ?? "/";
    var callback  = $"/Identity/Account/ExternalLogin/Callback?returnUrl={Uri.EscapeDataString(returnUrl)}";
    var props     = signInManager.ConfigureExternalAuthenticationProperties(provider, callback);
    return Results.Challenge(props, new[] { provider });
}).DisableAntiforgery();

app.MapGet("/Identity/Account/ExternalLogin/Callback", async (
    HttpContext http,
    string? returnUrl,
    SignInManager<AppUser> signInManager,
    UserManager<AppUser> userManager) =>
{
    returnUrl ??= "/";
    var info = await signInManager.GetExternalLoginInfoAsync();
    if (info is null)
        return Results.Redirect("/login?error=external");

    var signIn = await signInManager.ExternalLoginSignInAsync(
        info.LoginProvider, info.ProviderKey, isPersistent: true);

    if (signIn.Succeeded)
        return Results.Redirect(returnUrl);

    var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? "";
    if (string.IsNullOrWhiteSpace(email))
        return Results.Redirect("/login?error=noemail");

    var user = new AppUser { UserName = email, Email = email };
    var created = await userManager.CreateAsync(user);
    if (created.Succeeded)
    {
        await userManager.AddLoginAsync(user, info);
        await signInManager.SignInAsync(user, isPersistent: true);
        return Results.Redirect(returnUrl);
    }

    var existing = await userManager.FindByEmailAsync(email);
    if (existing is not null)
    {
        await userManager.AddLoginAsync(existing, info);
        await signInManager.SignInAsync(existing, isPersistent: true);
        return Results.Redirect(returnUrl);
    }

    return Results.Redirect("/login?error=external");
});

// ── Migrate DB + Seed admin ────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services    = scope.ServiceProvider;
    var db          = services.GetRequiredService<AppDbContextGames>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await db.Database.MigrateAsync();
    await SeedAdminAsync(userManager, roleManager);
}

app.Lifetime.ApplicationStopping.Register(() =>
    Log.Warning("Application stopping — flushing logs..."));

try { app.Run(); }
catch (Exception ex) { Log.Fatal(ex, "Host terminated unexpectedly"); }
finally { Log.CloseAndFlush(); }

// ── Seed helpers ──────────────────────────────────────────────────────────
static async Task SeedAdminAsync(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager)
{
    const string adminRole = "Admin";

    if (!await roleManager.RoleExistsAsync(adminRole))
        await roleManager.CreateAsync(new IdentityRole(adminRole));

    await EnsureAdminAsync(userManager, adminRole,
        email: "admin@local",
        username: "admin",
        password: "Admin123.");

    await EnsureAdminAsync(userManager, adminRole,
        email: "olsanskyvitek@gmail.com",
        username: "vitek",
        password: "Vitek575");
}

static async Task EnsureAdminAsync(
    UserManager<AppUser> userManager,
    string adminRole,
    string email,
    string username,
    string password)
{
    var user = await userManager.FindByEmailAsync(email);

    if (user is null)
    {
        user = new AppUser
        {
            UserName = username,
            Email = email,
            EmailConfirmed = true,
            IsAdmin = true
        };
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new Exception($"Failed to create user {email}: " +
                string.Join(", ", result.Errors.Select(e => e.Description)));
    }
    else if (!user.IsAdmin)
    {
        user.IsAdmin = true;
        await userManager.UpdateAsync(user);
    }

    if (!await userManager.IsInRoleAsync(user, adminRole))
        await userManager.AddToRoleAsync(user, adminRole);
}

public partial class Program { }

// ── No-op IEmailSender ────────────────────────────────────────────────────
file sealed class NoOpEmailSender : Microsoft.AspNetCore.Identity.UI.Services.IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
        => Task.CompletedTask;
}
