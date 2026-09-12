using MercenariesAndBeasts.Infrastructure;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using SharedServices;
using SharedServices.Models.TopEleven;
using System.Security.Claims;

namespace TopElevenStats.Web.Services;

/// <summary>
/// Zpřístupňuje Top Eleven účet přihlášeného uživatele. Každý uživatel vidí a mění
/// výhradně svá data — stránky nesmí sahat na <c>TopElevenAccounts</c> přímo.
/// </summary>
public sealed class TopElevenAccountAccessor
{
    private readonly AuthenticationStateProvider _auth;
    private readonly IDbContextFactory<AppDbContextGames> _dbFactory;

    public TopElevenAccountAccessor(
        AuthenticationStateProvider auth,
        IDbContextFactory<AppDbContextGames> dbFactory)
    {
        _auth = auth;
        _dbFactory = dbFactory;
    }

    /// <summary>Id přihlášeného uživatele, nebo <c>null</c> pokud nikdo přihlášen není.</summary>
    public async Task<string?> GetUserIdAsync()
    {
        var state = await _auth.GetAuthenticationStateAsync();
        var principal = state.User;
        if (principal?.Identity?.IsAuthenticated != true) return null;
        return principal.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    /// <summary>Účet přihlášeného uživatele, nebo <c>null</c> pokud ho ještě nemá.</summary>
    public async Task<TopElevenAccount?> GetAccountAsync()
    {
        var userId = await GetUserIdAsync();
        if (userId is null) return null;

        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.TopElevenAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.OwnerUserId == userId);
    }

    /// <summary>
    /// Guid účtu přihlášeného uživatele. Vrací <c>null</c>, když uživatel účet nemá —
    /// dotazy filtrované tímto Guidem pak musí vrátit prázdný výsledek, ne všechna data.
    /// </summary>
    public async Task<Guid?> GetAccountIdAsync() => (await GetAccountAsync())?.Guid;

    /// <summary>Vytvoří účet pro přihlášeného uživatele. Vyhodí, pokud už nějaký má.</summary>
    public async Task<TopElevenAccount> CreateAccountAsync(string name)
    {
        var userId = await GetUserIdAsync()
            ?? throw new InvalidOperationException("Účet lze vytvořit jen pro přihlášeného uživatele.");

        await using var db = await _dbFactory.CreateDbContextAsync();
        if (await db.TopElevenAccounts.AnyAsync(a => a.OwnerUserId == userId))
            throw new InvalidOperationException("Uživatel už účet má.");

        var account = new TopElevenAccount
        {
            Name = name,
            CurrentSeason = 1,
            OwnerUserId = userId
        };
        db.TopElevenAccounts.Add(account);
        await db.SaveChangesAsync();
        return account;
    }

    /// <summary>
    /// Ověří, že hráč patří do účtu přihlášeného uživatele. Brání přístupu na cizí
    /// záznam přes uhodnuté Guid v URL.
    /// </summary>
    public async Task<bool> OwnsPlayerAsync(Guid playerId)
    {
        var accountId = await GetAccountIdAsync();
        if (accountId is null) return false;

        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.TopElevenPlayers
            .AnyAsync(p => p.Guid == playerId && p.AccountId == accountId);
    }
}
