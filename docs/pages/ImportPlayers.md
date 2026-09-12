# ImportPlayers.razor
Route: `/import-players` — `[Authorize]` (od 2026-09-12; dřív Admin-only)
Popis: Import hráčů z CSV — vzorový soubor, náhled 20 řádků, počet chybných řádků, hromadné uložení.
Platforma: Web

## Hotovo ✅
- Stažení vzorového CSV, upload do 5 MB, náhled s počtem řádků
- Počítání chybných řádků, import přes `AddRange`
- Achievementy `FIRST_IMPORT`, `IMPORT_CSV`, `IMPORT_50`
- **2026-09-11:** hráči se importují do účtu přihlášeného uživatele (`TopElevenAccountAccessor`)

## Chybí / Rozpracováno ⚠️
- Vlastní ruční CSV parser, přestože `CsvHelper` 33.1.0 je v csproj a nikde se nepoužívá
- `content.Split('\n')` rozbije uvozovkované pole s odřádkováním
- **Import vždy přidává** — dvojí import stejného souboru zdvojí kádr (chybí upsert podle jména)
- Chybné řádky se jen spočítají, uživatel nevidí které ani proč
- Nelze importovat statistiky, jen hráče

## Návrhy na vylepšení 💡
- Režim importu: přidat / aktualizovat podle jména / nahradit kádr
- Tabulka chybných řádků s důvodem
- Import sezónních statistik

## Brainstorming poznámky
- `PERFECT_IMPORT` doplněn 2026-09-12; `IMPORT_5_TIMES` vyžaduje perzistentní čítač importů
