# AddStats.razor
Route: `/stats/add` — `[Authorize]` (od 2026-09-12; dřív Admin-only)
Popis: Zadání sezónních statistik jednoho hráče za aktuální sezónu účtu; existující záznam přepíše.
Platforma: Web

## Hotovo ✅
- Výběr hráče (GK nahoře), formulář ~13 polí, GK/outfield pole podle typu hráče
- Varování, že záznam pro sezónu už existuje a přepíše se
- **2026-09-11:** účet přes `TopElevenAccountAccessor`; formulář se otevře jen pro hráče vlastního účtu

## Chybí / Rozpracováno ⚠️
- **Zadávání po jednom hráči** — 20+ průchodů každou sezónu, největší UX brzda aplikace
- Žádná validace rozsahů (hodnocení 0–10, win% 0–100, záporné hodnoty projdou)
- ~~Neodemyká `STATS_*`~~ — vyřešeno 2026-09-12 přes `TopElevenAchievementEvaluator`
- Chybí přehled „kolik hráčů v sezóně ještě nemá statistiky“

## Návrhy na vylepšení 💡
- Hromadná mřížka: všichni hráči × sloupce statistik, jedno Uložit
- Import statistik z CSV
- Progress „12/22 hráčů zadáno“

## Brainstorming poznámky
- Mřížka musí respektovat GK vs. outfield sloupce — buď dvě tabulky, nebo prázdné buňky
