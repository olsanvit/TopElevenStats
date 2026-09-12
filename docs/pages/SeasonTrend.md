# SeasonTrend.razor
Route: `/seasons`
Popis: Vývoj kádru přes sezóny — kombinovaný graf a souhrnná tabulka po sezónách.
Platforma: Web

## Hotovo ✅
- Graf: sloupce góly/čistá konta, linky prům. OVR a prům. hodnocení
- Tabulka: počet hráčů, prům. OVR, hodnocení, góly/čistá konta, win%
- Přepínač účtů (zobrazí se jen při více účtech)
- **2026-09-11:** nabízí jen účty přihlášeného uživatele; zvolený účet se ověřuje proti tomuto seznamu (hodnota selectu přichází od klienta)

## Chybí / Rozpracováno ⚠️
- „Prům. OVR“ za sezónu počítá z aktuálního OVR hráče, ne z OVR v té sezóně → historicky nepravdivé
- Chybí filtr elitní/brankáři, srovnání dvou sezón, export

## Návrhy na vylepšení 💡
- Po zavedení historie OVR počítat průměr z `TopElevenSeasonStats.Ovr`

## Brainstorming poznámky
- `/seasons`, `/season`, `/season/report` → sloučit do jedné stránky se záložkami Vývoj | Report | Správa
- Uživatel má nyní max. 1 účet, přepínač účtů je prakticky mrtvý kód — ponechat pro případ více účtů na uživatele
