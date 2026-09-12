using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using SharedServices.Services;
using System.Security.Claims;

namespace TopElevenStats.Web.Services;

/// <summary>
/// Přepne scoped <see cref="AchievementService"/> interaktivního circuitu na per-user DB perzistenci.
/// </summary>
/// <remarks>
/// Stránky běží ve vlastním interaktivním circuitu s vlastním DI scope, takže
/// <c>InitForUserAsync</c> zavolaný z layoutu nebo stránky při HTTP requestu by v circuitu
/// neplatil a služba by zůstala na sdíleném souboru <c>achievements.json</c>. Circuit handler
/// běží dřív, než se komponenty vyrenderují, takže stránky dostanou už inicializovanou službu.
/// Ověřeno na MAB, kde přesně tahle past způsobila míchání odemčení mezi uživateli.
/// </remarks>
public sealed class AchievementCircuitInitializer(
    AuthenticationStateProvider authStateProvider,
    AchievementService achievements,
    DbAchievementStore store) : CircuitHandler
{
    private bool _initialized;

    public override async Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        if (_initialized) return;

        var state = await authStateProvider.GetAuthenticationStateAsync();
        var userId = state.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return;

        await achievements.InitForUserAsync(userId, store);
        _initialized = true;
    }
}
