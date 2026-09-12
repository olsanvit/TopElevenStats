# Home.razor
Route: `/`
Popis: Dashboard hráče — souhrnné dlaždice, distribuce OVR a tabulky brankářů a hráčů v poli za aktuální sezónu.
Platforma: Web (mobilní obdoba: Mobile/Home.razor)

## Hotovo ✅
- 4 dlaždice: počet hráčů, prům. OVR, elitní hráči, achievementy (odemčeno/celkem)
- Sloupcový graf distribuce OVR (ApexCharts)
- QuickLinkCards na Hráče, Statistiky, Achievementy
- Tabulka brankářů (čistá konta, karty, win% s/bez hráče) a hráčů v poli (góly, xG90, asistence)
- Achievementy `FIRST_VISIT_HOME`, `EARLY_BIRD`, `NIGHT_OWL`
- **2026-09-11:** účet se bere přes `TopElevenAccountAccessor` (jen vlastní účet přihlášeného uživatele)
- **2026-09-12:** při otevření dashboardu běží `TopElevenAchievementEvaluator`
- **2026-09-11:** opraven souběh dvou dotazů na jedné instanci DbContextu (`Task.WhenAll` z commitu 3d33535 → EF vyhazoval InvalidOperationException a dashboard ukazoval chybový toast)

## Chybí / Rozpracováno ⚠️
- Načítá všechny hráče i statistiky do paměti bez stránkování
- Hranice OVR skupin natvrdo 60–80+; hráč pod 60 se v grafu neobjeví
- `Theme = Mode.Dark` natvrdo — při světlém tématu z ThemePickeru je graf tmavý
- Tabulka hráčů v poli nemá odkaz na detail (brankáři ho mají)
- Nelze přepnout sezónu, jen aktuální; žádné řazení kliknutím na hlavičku

## Návrhy na vylepšení 💡
- Widget „největší zlepšení / propady OVR proti minulé sezóně“ (vyžaduje historii OVR)
- Pracovní seznam „hráči bez statistik v této sezóně“ s odkazem na zadání
- Věková pyramida kádru + upozornění na hráče 33+

## Brainstorming poznámky
- Tabulky duplikují `/season/report` — zvážit nahradit je top 5 + odkazem na report
- Grafy by měly číst téma z ThemeService (týká se 5 stránek najednou → sdílený helper pro ApexChartOptions)
