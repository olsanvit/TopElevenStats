using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SharedServices;

namespace TopElevenStats.Web;

/// <summary>
/// Provides a design-time factory for <see cref="AppDbContextGames"/> used by EF Core tooling
/// (e.g., <c>dotnet ef migrations</c>). It configures a hard-coded development connection string
/// so that migrations can be created and applied without a running application host.
/// </summary>
public class AppDbContextGamesDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContextGames>
{
    /// <summary>
    /// Creates and configures a new <see cref="AppDbContextGames"/> instance for design-time use.
    /// The returned context targets the development PostgreSQL database and registers
    /// <c>SharedServices</c> as the migrations assembly.
    /// </summary>
    /// <param name="args">Command-line arguments forwarded by EF Core tooling; not used.</param>
    /// <returns>A fully configured <see cref="AppDbContextGames"/> instance ready for migration operations.</returns>
    public AppDbContextGames CreateDbContext(string[] args)
    {
        const string cs =
            "Host=100.99.239.94;Port=5432;Database=topEleven;Username=roundnet;Password=kindred;" +
            "Pooling=true;Timeout=50;Command Timeout=120;Ssl Mode=Disable";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContextGames>();
        optionsBuilder.UseNpgsql(cs, o =>
        {
            o.MigrationsAssembly("SharedServices");
        });

        return new AppDbContextGames(optionsBuilder.Options);
    }
}
