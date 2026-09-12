# Season.razor
Route: `/season`
Popis: Správa sezóny — aktuální číslo sezóny, posun +1, přehled sezón s počty záznamů a mazání celé sezóny.
Platforma: Web

## Hotovo ✅
- Zobrazení aktuální sezóny, tlačítko „Nová sezóna +1“
- Tabulka sezón s počtem hráčů se statistikami, mazání sezóny přes `ConfirmDialog`
- **2026-09-11:** účet přes `TopElevenAccountAccessor`
- **2026-09-12:** posun sezóny má potvrzovací dialog, odemyká `SEASON_RESET`/`FIRST_SEASON`

## Chybí / Rozpracováno ⚠️
- Přechod sezóny nic dalšího nedělá: nezvýší věk, nearchivuje OVR, nenabídne kontrolu chybějících statistik
- ~~Neodemyká `SEASON_*`~~ — vyřešeno 2026-09-12 (potvrzovací dialog + evaluator)

## Návrhy na vylepšení 💡
- Průvodce přechodem sezóny: kontrola úplnosti → archiv OVR → věk +1 → posun
- Tlačítko „zpět o sezónu“ pro vlastníka (ne jen admin)

## Brainstorming poznámky
- Kandidát na záložku „Správa“ ve sloučené stránce sezón
