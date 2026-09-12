using Microsoft.EntityFrameworkCore;
using SharedServices;
using SharedServices.Models.Achievement;
using SharedServices.Services;

namespace TopElevenStats.Web.Services;

/// <summary>
/// Ukládá odemčené achievementy po uživatelích do DB. Nahrazuje výchozí souborovou
/// perzistenci <c>achievements.json</c>, která byla společná všem uživatelům a navíc
/// se ztrácela při každém redeployi kontejneru (soubor je v ContentRootu, ne ve volume).
/// </summary>
public sealed class DbAchievementStore : IAchievementStore
{
    private readonly IDbContextFactory<AppDbContextGames> _factory;

    public DbAchievementStore(IDbContextFactory<AppDbContextGames> factory) => _factory = factory;

    public async Task<List<UnlockedRecord>> LoadAsync(string userId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var rows = await db.UserAchievements
            .Where(a => a.UserId == userId)
            .ToListAsync();
        return rows.Select(r => new UnlockedRecord { Key = r.Key, UnlockedAt = r.UnlockedAt }).ToList();
    }

    public async Task SaveAsync(string userId, List<UnlockedRecord> records)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var existingKeys = await db.UserAchievements
            .Where(a => a.UserId == userId)
            .Select(a => a.Key)
            .ToHashSetAsync();

        var newRows = records
            .Where(r => !existingKeys.Contains(r.Key))
            .Select(r => new UserAchievement { UserId = userId, Key = r.Key, UnlockedAt = r.UnlockedAt });

        db.UserAchievements.AddRange(newRows);
        await db.SaveChangesAsync();
    }
}
