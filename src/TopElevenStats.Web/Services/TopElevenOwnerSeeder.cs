using MercenariesAndBeasts.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedServices;

namespace TopElevenStats.Web.Services;

/// <summary>
/// Jednorázově přiřadí vlastníka účtům, které vznikly před zavedením vlastnictví
/// (<c>OwnerUserId IS NULL</c>). Bez toho by se stará data po nasazení nikomu nezobrazila.
/// </summary>
public static class TopElevenOwnerSeeder
{
    /// <summary>
    /// Osiřelé účty přiřadí seed adminovi (konfigurace <c>Admin:Email</c>). Když admin
    /// neexistuje nebo osiřelý účet žádný není, neudělá nic.
    /// </summary>
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var logger = sp.GetService<ILoggerFactory>()?.CreateLogger(nameof(TopElevenOwnerSeeder));

        try
        {
            var dbFactory = sp.GetRequiredService<IDbContextFactory<AppDbContextGames>>();
            await using var db = await dbFactory.CreateDbContextAsync();

            var orphans = await db.TopElevenAccounts
                .Where(a => a.OwnerUserId == null)
                .ToListAsync();
            if (orphans.Count == 0) return;

            var userManager = sp.GetRequiredService<UserManager<AppUser>>();
            var adminEmail = config["Admin:Email"] ?? "olsanskyvitek@gmail.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin is null)
            {
                logger?.LogWarning(
                    "{Count} Top Eleven účtů nemá vlastníka, ale admin '{Email}' neexistuje — účty zůstávají osiřelé.",
                    orphans.Count, adminEmail);
                return;
            }

            foreach (var account in orphans)
                account.OwnerUserId = admin.Id;

            await db.SaveChangesAsync();
            logger?.LogInformation(
                "{Count} Top Eleven účtů bez vlastníka přiřazeno adminovi '{Email}'.",
                orphans.Count, adminEmail);
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Přiřazení vlastníků Top Eleven účtů přeskočeno.");
        }
    }
}
