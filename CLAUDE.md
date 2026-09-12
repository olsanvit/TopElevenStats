# TopElevenStats — CLAUDE.md

## Co projekt dělá

TopElevenStats je webová aplikace pro správu hráčů a statistik z fotbalové manažerské hry Top Eleven. Umožňuje:

- Evidovat hráče (jméno, OVR, věk, role, spec. schopnost, elita, brankář, občanství)
- Zobrazovat sezónní statistiky (hodnocení, góly, asistence, čistá konta, výhra s/bez hráče…)
- Importovat a exportovat hráče ve formátu CSV
- Srovnávat hráče a spravovat sezóny

## Architektura

- **Framework:** .NET 10, Blazor Server (`@rendermode InteractiveServer`)
- **ORM:** Entity Framework Core
- **DB:** PostgreSQL (Npgsql) — produkce `TopEleven` na pg16 (QNAP); dev přes `DefaultConnection1QNAP`
- **Auth:** ASP.NET Core Identity + Google OAuth, `[Authorize]` na stránkách, `[Authorize(Roles = "Admin")]` na admin sekci
- **UI knihovny:** Bootstrap 5, Bootstrap Icons (`bi-*`), ApexCharts (`Blazor-ApexCharts`), Blazored.Modal
- **Achievementy:** `Achievements/TopElevenAchievements.cs` (100 definic) + `AchievementService` ze SharedServices
- **Mobile:** `src/TopElevenStats.Mobile` (MAUI) — jen Home a seznam hráčů, read-only

## Klíčové modely (namespace `SharedServices.Models.TopEleven`)

### `TopElevenPlayer : BaseGuid`
| Vlastnost | Typ | Popis |
|---|---|---|
| `AccountId` | `Guid` | FK na účet |
| `Name` | `string` | Jméno hráče |
| `Ovr` | `int` | Celkové hodnocení |
| `Age` | `int` | Věk |
| `Roles` | `string` | Role oddělené čárkou (např. `GK`, `AML,ML`) |
| `IsGoalkeeper` | `bool` | Je brankář |
| `IsElite` | `bool` | Je elitní hráč |
| `SpecialAbility` | `string?` | Popis spec. schopnosti |
| `Citizenship` | `string?` | Občanství |

### `TopElevenAccount : BaseGuid`
Reprezentuje uživatelský účet v Top Eleven. Obsahuje `Name`, `CurrentSeason` a `OwnerUserId`
(FK na `AspNetUsers.Id`, nullable kvůli datům z doby před zavedením vlastnictví).
Název je unikátní v rámci vlastníka — index `(OwnerUserId, Name)`.

### `TopElevenSeasonStats`
Sezónní statistiky hráče: zápasy, góly, asistence, hodnocení, karty, čistá konta, win-ratio.

## DbContext

`AppDbContextGames` — registrován přes `IDbContextFactory<AppDbContextGames>`.

Používej vždy `await using var db = await DbFactory.CreateDbContextAsync();` — nikdy neinjektuj DbContext přímo (Blazor Server lifetime).

## SharedServices submodul

Sdílené komponenty a modely jsou ve `src/SharedServices/SharedServices/`:

| Komponenta/Service | Použití |
|---|---|
| `Paginator` | Stránkování (`TotalItems`, `PageSize`, `CurrentPage`, `OnPageChanged`) |
| `ToastService` | Notifikace — `Toast.ShowSuccess(...)`, `Toast.ShowError(...)` |
| `ConfirmDialog` | Potvrzovací dialog před smazáním (`await _confirmDialog.ShowAsync(...)`) |
| `PageLoadingSpinner` | Spinner během načítání dat |
| `ThemePicker` | Přepínač světlého/tmavého tématu (v NavMenu) |

## Hlavní stránky

Všechny jsou v `src/TopElevenStats.Web/Components/Pages/`. Podrobný rozbor každé stránky
(hotovo / chybí / návrhy) je v `docs/pages/<Stranka>.md`.

| Route | Soubor | Popis |
|---|---|---|
| `/` | `Home.razor` | Dashboard: dlaždice, OVR distribuce, tabulky GK a hráčů v poli |
| `/players` | `Players.razor` | Seznam hráčů se stránkováním, filtry, export CSV, založení účtu |
| `/players/{id}` | `PlayerDetail.razor` | Profil hráče, kariérní průměry, vývoj přes sezóny |
| `/import-players` | `ImportPlayers.razor` | Import hráčů z CSV (Admin) |
| `/stats` | `Stats.razor` | Grafy statistik jednoho hráče (ApexCharts) |
| `/stats/add` | `AddStats.razor` | Zadání sezónních statistik (Admin) |
| `/compare` | `Compare.razor` | Srovnání dvou hráčů + radar |
| `/seasons` | `SeasonTrend.razor` | Vývoj kádru přes sezóny |
| `/season` | `Season.razor` | Správa sezóny (posun +1, mazání sezóny) |
| `/season/report` | `SeasonReport.razor` | Žebříček hráčů za sezónu, export CSV |
| `/achievements` | `AchievementsPage.razor` | Mřížka achievementů |
| `/admin` | `Admin/AdminDashboard.razor` | Admin přehled (Admin) |
| `/Error`, `/not-found` | `Error.razor`, `NotFound.razor` | Chybové stránky |

## Vlastnictví dat (multi-tenancy)

Každý účet patří právě jednomu uživateli. Stránky proto **nikdy** nesmí sáhnout na
`db.TopElevenAccounts` přímo (dřívější `FirstOrDefaultAsync()` bralo „první účet v DB“,
takže druhý uživatel viděl cizí data).

- Účet vždy přes `@inject TopElevenAccountAccessor AccountAccessor`
  → `GetAccountAsync()` / `GetAccountIdAsync()` / `CreateAccountAsync(name)`
- Každý dotaz na hráče a statistiky musí být filtrovaný: `.Where(p => p.AccountId == accountId)`
  nebo `.Where(s => s.Player.AccountId == accountId)`
- Guid přicházející od klienta (route parametr, hodnota `<select>`) se **musí ověřit** proti
  vlastnímu účtu — `OwnsPlayerAsync(id)`, nebo dohledáním v už načteném seznamu
- Výjimka: `Admin/AdminDashboard.razor` záměrně vidí data všech účtů

## Migrace

Kontext je `AppDbContextGames`, migrace patří do
`src/SharedServices/SharedServices/Migrations/AppDbContextGamesMigrations`:

```
dotnet ef migrations add <Nazev> --context AppDbContextGames \
  --project src/SharedServices/SharedServices --startup-project src/TopElevenStats.Web \
  --output-dir Migrations/AppDbContextGamesMigrations
```

Na přetíženém Macu to trvá i přes 10 minut (buildí celý submodul). Repo má precedens
ručně psaných migrací s atributy `[DbContext]` + `[Migration]` a bez `.Designer.cs`
(např. `AddMustChangePassword`, `AddTopElevenAccountOwner`) — u jednoduché změny je to rychlejší,
ale pak je nutné ručně srovnat i `AppDbContextGamesModelSnapshot.cs`.

Model i migrace leží v submodulu SharedServices: **nejdřív commit a push v submodulu,
teprve pak parent** — `git submodule update --remote` by rozpracované změny zahodil.

## Konvence a vzory

- **Stránkování:** vždy přes komponentu `<Paginator>`, parametr `OnPageChanged` resetuje `_page` a volá `LoadXyzAsync()`
- **Toasty:** `Toast.ShowSuccess("Titulek", "Detail")` / `Toast.ShowError(...)`
- **Smazání:** vždy přes `<ConfirmDialog @ref="_confirmDialog" />` + `await _confirmDialog.ShowAsync("Nadpis", "Otázka")`
- **Načítání:** `bool _loading = true;` + `<PageLoadingSpinner />` v šabloně
- **Filtry/search:** server-side pomocí `.Where()` na IQueryable před `.CountAsync()` a stránkováním; po změně filtru vždy reset `_page = 1`
- **Grafy:** ApexCharts — `<ApexChart TItem="...">` + `<ApexPointSeries ... SeriesType="SeriesType.Bar"/>`
- **CSV formát:** `Jméno,OVR,Věk,Role,Spec. schopnost,Elitní,Brankář,Občanství`

## Formát CSV pro import/export

```
Jméno,OVR,Věk,Role,Spec. schopnost,Elitní,Brankář,Občanství
"Novák Jan",85,24,"ST","Finišer",True,False,"CZE"
```

Hodnoty s čárkou nebo uvozovkami jsou obaleny do `"..."`, uvozovky uvnitř jsou zdvojeny (`""`).

## Bezpečnost

- Repozitář je **veřejný** (github.com/olsanvit/TopElevenStats) — nikdy do něj nesmí živé credentials.
- `appsettings.Production.json` je v `.gitignore`; pokud se znovu objeví v `git ls-files`, je to incident.
- Dev/prod connection stringy patří do User Secrets (`UserSecretsId: top-eleven-stats-dev`) nebo env proměnných.
