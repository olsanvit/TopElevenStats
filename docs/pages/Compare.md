# Compare.razor
Route: `/compare`
Popis: Srovnání dvou hráčů — kariérní souhrn s vítězem v každé kategorii, radar a tabulka společných sezón.
Platforma: Web

## Hotovo ✅
- Dva selecty, kariérní souhrn (OVR, hodnocení, zápasy, góly/čistá konta, asistence, win%, věk)
- Radar chart s normalizací na 0–100
- Tabulka společných sezón se zvýrazněním lepší hodnoty
- **2026-09-11:** hráči jen z vlastního účtu; cizí Guid z klienta se ignoruje
- **2026-09-12:** opraven překlep „Asistenece“; odemyká `FIRST_COMPARE` a `COMPARE_SEASONS`

## Chybí / Rozpracováno ⚠️
- Jen 2 hráči; selecty bez typeaheadu
- Chybí permalink `/compare/{a}/{b}`
- Normalizace radaru natvrdo (`NormGoals` dělí 15 → brankář s 20 čistými konty useknutý)


## Návrhy na vylepšení 💡
- Srovnání 3–4 hráčů (výběr koho prodat)
- „Hráč vs. průměr jeho pozice v kádru“
- Export srovnání do CSV

## Brainstorming poznámky
- Normalizovat radar vůči maximu v kádru místo pevných konstant
