# Stats.razor
Route: `/stats`
Popis: Statistiky jednoho hráče — výběr v selectu, dva grafy přes sezóny a široká tabulka všech sloupců.
Platforma: Web

## Hotovo ✅
- Graf hodnocení + win% s hráčem, graf gólů/čistých kont + asistencí
- Tabulka: hodnocení, zápasy, góly/xG90 nebo čistá konta, asistence, klíčové přihrávky, zranění, karty, win% s/bez, týmový %
- **2026-09-11:** seznam hráčů jen z vlastního účtu; statistiky se načtou jen pro hráče z tohoto seznamu (hodnota selectu přichází od klienta)

## Chybí / Rozpracováno ⚠️
- Select bez vyhledávání — při 100+ hráčích nepoužitelný (`Blazored.Typeahead` je v csproj, nevyužitý)
- Silně se překrývá s `/players/{id}`
- Grafy natvrdo tmavé

## Návrhy na vylepšení 💡
- **Sloučit do PlayerDetail** jako záložku „Detailní statistiky“ a položku z menu odebrat
- Typeahead výběr hráče

## Brainstorming poznámky
- Pokud zůstane samostatně: předvybrat hráče z query stringu `?player=<guid>`
