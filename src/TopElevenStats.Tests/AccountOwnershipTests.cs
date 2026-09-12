using FluentAssertions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using SharedServices;
using SharedServices.Models.TopEleven;
using System.Security.Claims;
using TopElevenStats.Web.Services;

namespace TopElevenStats.Tests;

/// <summary>
/// Hlídá vlastnictví dat. Dřív aplikace brala „první účet v DB“, takže druhý registrovaný
/// uživatel viděl a mohl mazat cizí kádr — tyhle testy tu regresi odchytí.
/// </summary>
public class AccountOwnershipTests
{
    private const string UserA = "user-a";
    private const string UserB = "user-b";

    private static readonly Guid AccountA = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid AccountB = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid PlayerOfA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid PlayerOfB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    /// <summary>Dva uživatelé, každý s jedním účtem a jedním hráčem.</summary>
    private static TestDbContextFactory SeededFactory()
    {
        var options = new DbContextOptionsBuilder<AppDbContextGames>()
            .UseInMemoryDatabase($"ownership-{Guid.NewGuid()}")
            .Options;

        var factory = new TestDbContextFactory(options);
        using var db = factory.CreateDbContext();
        db.TopElevenAccounts.AddRange(
            new TopElevenAccount { Guid = AccountA, Name = "Kádr A", OwnerUserId = UserA },
            new TopElevenAccount { Guid = AccountB, Name = "Kádr B", OwnerUserId = UserB });
        db.TopElevenPlayers.AddRange(
            new TopElevenPlayer { Guid = PlayerOfA, Name = "Hráč A", AccountId = AccountA },
            new TopElevenPlayer { Guid = PlayerOfB, Name = "Hráč B", AccountId = AccountB });
        db.SaveChanges();
        return factory;
    }

    private static TopElevenAccountAccessor AccessorFor(string? userId, TestDbContextFactory factory) =>
        new(new FakeAuthStateProvider(userId), factory);

    [Fact]
    public async Task GetAccountAsync_ReturnsOnlyOwnAccount()
    {
        var factory = SeededFactory();

        var account = await AccessorFor(UserA, factory).GetAccountAsync();

        account.Should().NotBeNull();
        account!.Guid.Should().Be(AccountA);
        account.Name.Should().Be("Kádr A");
    }

    [Fact]
    public async Task GetAccountAsync_ForUserWithoutAccount_ReturnsNull()
    {
        var factory = SeededFactory();

        var account = await AccessorFor("user-without-account", factory).GetAccountAsync();

        account.Should().BeNull();
    }

    [Fact]
    public async Task GetAccountAsync_WhenNotAuthenticated_ReturnsNull()
    {
        var factory = SeededFactory();

        var account = await AccessorFor(null, factory).GetAccountAsync();

        account.Should().BeNull();
    }

    [Fact]
    public async Task GetAccountIdAsync_ReturnsDifferentIdPerUser()
    {
        var factory = SeededFactory();

        var idA = await AccessorFor(UserA, factory).GetAccountIdAsync();
        var idB = await AccessorFor(UserB, factory).GetAccountIdAsync();

        idA.Should().Be(AccountA);
        idB.Should().Be(AccountB);
        idA.Should().NotBe(idB!.Value);
    }

    [Fact]
    public async Task OwnsPlayerAsync_ForOwnPlayer_IsTrue()
    {
        var factory = SeededFactory();

        var owns = await AccessorFor(UserA, factory).OwnsPlayerAsync(PlayerOfA);

        owns.Should().BeTrue();
    }

    [Fact]
    public async Task OwnsPlayerAsync_ForForeignPlayer_IsFalse()
    {
        var factory = SeededFactory();

        // Uhodnuté Guid cizího hráče v URL nesmí projít
        var owns = await AccessorFor(UserA, factory).OwnsPlayerAsync(PlayerOfB);

        owns.Should().BeFalse();
    }

    [Fact]
    public async Task OwnsPlayerAsync_WhenNotAuthenticated_IsFalse()
    {
        var factory = SeededFactory();

        var owns = await AccessorFor(null, factory).OwnsPlayerAsync(PlayerOfA);

        owns.Should().BeFalse();
    }

    [Fact]
    public async Task CreateAccountAsync_StampsOwner()
    {
        var factory = SeededFactory();

        var created = await AccessorFor("new-user", factory).CreateAccountAsync("Nový kádr");

        created.OwnerUserId.Should().Be("new-user");
        created.CurrentSeason.Should().Be(1);
    }

    [Fact]
    public async Task CreateAccountAsync_WhenUserAlreadyHasAccount_Throws()
    {
        var factory = SeededFactory();

        var act = () => AccessorFor(UserA, factory).CreateAccountAsync("Druhý kádr");

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task CreateAccountAsync_WhenNotAuthenticated_Throws()
    {
        var factory = SeededFactory();

        var act = () => AccessorFor(null, factory).CreateAccountAsync("Kádr bez uživatele");

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task SameAccountName_IsAllowedForDifferentOwners()
    {
        var factory = SeededFactory();

        // Výchozí název „Můj účet“ nabízí formulář všem — proto je unikátní jen v rámci vlastníka
        var first = await AccessorFor("user-c", factory).CreateAccountAsync("Můj účet");
        var second = await AccessorFor("user-d", factory).CreateAccountAsync("Můj účet");

        first.Guid.Should().NotBe(second.Guid);
        first.OwnerUserId.Should().NotBe(second.OwnerUserId);
    }

    private sealed class TestDbContextFactory : IDbContextFactory<AppDbContextGames>
    {
        private readonly DbContextOptions<AppDbContextGames> _options;
        public TestDbContextFactory(DbContextOptions<AppDbContextGames> options) => _options = options;
        public AppDbContextGames CreateDbContext() => new(_options);
    }

    /// <summary>Vrací pevný stav přihlášení; <c>null</c> userId = nepřihlášený návštěvník.</summary>
    private sealed class FakeAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ClaimsPrincipal _principal;

        public FakeAuthStateProvider(string? userId) =>
            _principal = userId is null
                ? new ClaimsPrincipal(new ClaimsIdentity())
                : new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(ClaimTypes.NameIdentifier, userId)], authenticationType: "Test"));

        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
            Task.FromResult(new AuthenticationState(_principal));
    }
}
