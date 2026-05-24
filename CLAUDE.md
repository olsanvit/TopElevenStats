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
- **DB:** SQL Server (nebo SQLite v dev prostředí)
- **Auth:** ASP.NET Core Identity s `[Authorize]` atributem na stránkách
- **UI knihovny:** Bootstrap 5, Bootstrap Icons (`bi-*`), ApexCharts (`ApexCharts.Blazor`), Blazored.Modal

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
Reprezentuje uživatelský účet v Top Eleven. Obsahuje `Name` a `CurrentSeason`.

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

| Route | Soubor | Popis |
|---|---|---|
| `/` | `Home.razor` | Dashboard: sezónní statistiky + OVR distribuce (ApexCharts) |
| `/players` | `Players.razor` | Seznam hráčů se stránkováním, filtry (jméno, elita, brankář), export CSV |
| `/import-players` | `ImportPlayers.razor` | Import hráčů z CSV souboru |
| `/stats` | `Stats.razor` | Grafy statistik (ApexCharts) |
| `/stats/add` | `Stats/Add.razor` | Přidání sezónních statistik |
| `/compare` | `Compare.razor` | Srovnání hráčů |
| `/season` | `Season.razor` | Správa sezóny |

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
