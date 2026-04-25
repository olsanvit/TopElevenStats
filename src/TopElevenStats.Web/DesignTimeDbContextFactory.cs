using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SharedServices;

namespace TopElevenStats.Web;

public class AppDbContextTopElevenDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContextTopEleven>
{
    public AppDbContextTopEleven CreateDbContext(string[] args)
    {
        const string cs =
            "Host=100.99.239.94;Port=5432;Database=topEleven;Username=roundnet;Password=kindred;" +
            "Pooling=true;Timeout=50;Command Timeout=120;Ssl Mode=Disable";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContextTopEleven>();
        optionsBuilder.UseNpgsql(cs, o =>
        {
            o.MigrationsAssembly("SharedServices");
        });

        return new AppDbContextTopEleven(optionsBuilder.Options);
    }
}
