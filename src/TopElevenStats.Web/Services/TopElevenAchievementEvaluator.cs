using Microsoft.EntityFrameworkCore;
using SharedServices;
using SharedServices.Services;

namespace TopElevenStats.Web.Services;

/// <summary>
/// Dopočítává achievementy, které vyplývají ze stavu kádru a statistik (počty hráčů,
/// elitních, brankářů, sezón…). Bez toho by se odemykaly jen ty, na které někdo pamatoval
/// přímo v obsluze tlačítka — a většina definic by zůstala nedosažitelná.
/// Volá se po každé změně dat a při otevření dashboardu.
/// </summary>
public sealed class TopElevenAchievementEvaluator
{
    private readonly AchievementService _achievements;
    private readonly IDbContextFactory<AppDbContextGames> _dbFactory;
    private readonly TopElevenAccountAccessor _accounts;

    public TopElevenAchievementEvaluator(
        AchievementService achievements,
        IDbContextFactory<AppDbContextGames> dbFactory,
        TopElevenAccountAccessor accounts)
    {
        _achievements = achievements;
        _dbFactory = dbFactory;
        _accounts = accounts;
    }

    /// <summary>
    /// Projde stav účtu přihlášeného uživatele a odemkne vše, na co už dosáhl.
    /// Chybu záměrně polyká — achievement nikdy nesmí shodit stránku s daty.
    /// </summary>
    public async Task EvaluateAsync()
    {
        try
        {
            var accountId = await _accounts.GetAccountIdAsync();
            if (accountId is null) return;

            await using var db = await _dbFactory.CreateDbContextAsync();

            var players = await db.TopElevenPlayers
                .Where(p => p.AccountId == accountId)
                .Select(p => new { p.Guid, p.Ovr, p.IsElite, p.IsGoalkeeper })
                .ToListAsync();

            var stats = await db.TopElevenSeasonStats
                .Where(s => s.Player.AccountId == accountId)
                .Select(s => new
                {
                    s.PlayerId, s.Season, s.AvgRating, s.MatchesPlayed, s.Goals, s.Assists,
                    s.CleanSheets, s.RedCards, s.WinRatioWith
                })
                .ToListAsync();

            // ── Kádr ──────────────────────────────────────────────────────────
            await CheckIf(players.Count >= 5, "PLAYERS_5");
            await CheckIf(players.Count >= 10, "PLAYERS_10");
            await CheckIf(players.Count >= 25, "PLAYERS_25");
            await CheckIf(players.Count >= 50, "PLAYERS_50");
            await CheckIf(players.Count >= 100, "PLAYERS_100");
            await CheckIf(players.Count >= 200, "PLAYERS_200");
            await CheckIf(players.Count >= 500, "PLAYERS_500");

            // ── OVR ───────────────────────────────────────────────────────────
            if (players.Count > 0)
            {
                var maxOvr = players.Max(p => p.Ovr);
                await CheckIf(maxOvr >= 70, "OVR_70");
                await CheckIf(maxOvr >= 75, "OVR_75");
                await CheckIf(maxOvr >= 80, "OVR_80");
                await CheckIf(maxOvr >= 85, "OVR_85");
                await CheckIf(maxOvr >= 90, "OVR_90");
                await CheckIf(maxOvr >= 95, "OVR_95");
                await CheckIf(maxOvr >= 100, "OVR_100");

                var avgOvr = players.Average(p => p.Ovr);
                await CheckIf(avgOvr >= 75, "AVG_OVR_75");
                await CheckIf(avgOvr >= 80, "AVG_OVR_80");

                await CheckIf(players.Count(p => p.Ovr >= 80) >= 5, "TOP5_OVR_80");
                await CheckIf(players.Count(p => p.Ovr >= 85) >= 5, "TOP5_OVR_85");
                await CheckIf(players.Count(p => p.Ovr >= 80) >= 10, "TOP10_OVR_80");
            }

            // ── Elite ─────────────────────────────────────────────────────────
            var elite = players.Count(p => p.IsElite);
            await CheckIf(elite >= 1, "FIRST_ELITE");
            await CheckIf(elite >= 5, "ELITE_5");
            await CheckIf(elite >= 10, "ELITE_10");
            await CheckIf(elite >= 25, "ELITE_25");
            await CheckIf(elite >= 50, "ELITE_50");
            await CheckIf(players.Count >= 10 && elite == players.Count, "ALL_ELITE");
            await CheckIf(players.Count >= 20 && elite * 2 >= players.Count, "ELITE_RATE_50");

            // ── Brankáři ──────────────────────────────────────────────────────
            var keepers = players.Where(p => p.IsGoalkeeper).ToList();
            await CheckIf(keepers.Count >= 1, "FIRST_GK");
            await CheckIf(keepers.Count >= 3, "GK_3");
            await CheckIf(keepers.Count >= 5, "GK_5");
            await CheckIf(keepers.Count >= 10, "GK_10");
            await CheckIf(keepers.Any(p => p.Ovr >= 80), "GK_OVR_80");
            await CheckIf(keepers.Any(p => p.Ovr >= 85), "GK_OVR_85");
            await CheckIf(keepers.Count(p => p.IsElite) >= 1, "ELITE_GK");
            await CheckIf(keepers.Count(p => p.IsElite) >= 3, "GK_ELITE_MULTIPLE");

            // ── Statistiky ────────────────────────────────────────────────────
            if (stats.Count > 0)
            {
                await _achievements.CheckAsync("FIRST_STATS");
                await CheckIf(stats.Any(s => (s.Goals ?? 0) > 0), "STATS_GOAL");
                await CheckIf(stats.Any(s => s.Assists > 0), "STATS_ASSIST");
                await CheckIf(stats.Any(s => s.RedCards > 0), "STATS_CARD");
                await CheckIf(stats.Any(s => (s.CleanSheets ?? 0) > 0), "STATS_CLEAN_SHEET");
                await CheckIf(stats.Any(s => s.AvgRating >= 9m), "STATS_RATING_9");
                await CheckIf(stats.Any(s => s.WinRatioWith > 50), "STATS_WIN");
                await CheckIf(stats.Any(s => s.WinRatioWith < 50), "STATS_LOSS");
                await CheckIf(stats.Any(s => s.WinRatioWith == 50), "STATS_DRAW");
                await CheckIf(stats.Any(s => s.WinRatioWith >= 70), "HIGH_WINRATE");
                await CheckIf(stats.Any(s => s.WinRatioWith == 100 && s.MatchesPlayed >= 10), "STATS_PERFECT_SEASON");

                await CheckIf(stats.Count >= 5, "STATS_5");
                await CheckIf(stats.Count >= 10, "STATS_10");
                await CheckIf(stats.Count >= 25, "STATS_25");

                // Hráč se 100+ odehranými zápasy napříč sezónami
                await CheckIf(
                    stats.GroupBy(s => s.PlayerId).Any(g => g.Sum(s => s.MatchesPlayed) >= 100),
                    "STATS_100_GAMES");

                // Hráč se záznamy ve více než 5 sezónách
                await CheckIf(
                    stats.GroupBy(s => s.PlayerId).Any(g => g.Select(s => s.Season).Distinct().Count() > 5),
                    "LONG_CAREER");

                var seasons = stats.Select(s => s.Season).Distinct().Count();
                await CheckIf(seasons >= 3, "SEASON_3");
                await CheckIf(seasons >= 5, "SEASON_5");
                await CheckIf(seasons >= 10, "SEASON_10");
                await CheckIf(seasons >= 20, "SEASON_20");
                await CheckIf(seasons >= 2, "BEST_SEASON");

                // Kompletní sezóna: statistiky má každý hráč kádru (min. 5 hráčů)
                await CheckIf(
                    players.Count >= 5 &&
                    stats.GroupBy(s => s.Season)
                         .Any(g => g.Select(s => s.PlayerId).Distinct().Count() >= players.Count),
                    "ALL_STATS_IN");
            }

            // ── Meta (musí být až po ostatních) ───────────────────────────────
            var unlocked = _achievements.GetAllStatus().Count(s => s.Unlock != null);
            await CheckIf(unlocked >= 50, "ALL_ACHIEVEMENTS");
            await CheckIf(_achievements.TotalPoints >= 1000, "TOTAL_1000_PTS");
        }
        catch
        {
            // Vyhodnocení achievementů je vedlejší efekt — selhání nesmí ovlivnit stránku.
        }
    }

    private Task CheckIf(bool condition, string key) =>
        condition ? _achievements.CheckAsync(key) : Task.CompletedTask;
}
