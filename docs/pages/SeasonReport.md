# SeasonReport.razor
Route: `/season/report`
Popis: Žebříček hráčů za zvolenou sezónu (nebo všechny) podle hodnocení, souhrnné dlaždice, export CSV.
Platforma: Web

## Hotovo ✅
- Filtr sezóny / všechny sezóny, 4 souhrnné dlaždice
- Žebříček s pořadím, barevné hodnocení, odkazy na detail
- Export CSV
- **2026-09-11:** sezóny i řádky reportu jen z vlastního účtu (dříve se míchali hráči všech účtů)
- **2026-09-12:** CSV escapuje role s čárkou; export odemyká `STATS_EXPORT`/`EXPORT_STATS`

## Chybí / Rozpracováno ⚠️
- Řazení natvrdo podle hodnocení
- Chybí MVP sezóny (`SEASON_MVP` na to čeká), žádný graf


## Návrhy na vylepšení 💡
- Řazení kliknutím na hlavičky, zvýraznění MVP, top střelec, nejlepší brankář

## Brainstorming poznámky
- Kandidát na záložku „Report“ ve sloučené stránce sezón
