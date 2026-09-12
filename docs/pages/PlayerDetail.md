# PlayerDetail.razor
Route: `/players/{Id:guid}`
Popis: Profil hráče — karta s údaji, kariérní průměry, graf vývoje přes sezóny a tabulka sezón.
Platforma: Web

## Hotovo ✅
- Profilová karta (OVR, věk, role, spec. schopnost, elitní, GK, občanství)
- Kariérní průměry a součty (hodnocení, zápasy, góly, asistence, karty)
- Kombinovaný graf hodnocení (linka) + góly (sloupce), od 2 sezón
- Tabulka sezón s barevným hodnocením
- **2026-09-12:** detail nejlepšího hráče odemyká `MOST_OVR_PLAYER`
- **2026-09-11:** hráč se načte jen pokud patří do účtu přihlášeného uživatele — cizí Guid v URL vrací „Hráč nenalezen“ (dříve IDOR: kdokoli se znalostí Guid viděl cizího hráče)

## Chybí / Rozpracováno ⚠️
- Tlačítka Upravit / Smazat / Porovnat s…
- Zkratka „zadat statistiky za aktuální sezónu“ pro tohoto hráče
- Graf neukazuje asistence ani win%
- Historie OVR neexistuje (OVR je jedno přepisované pole na hráči)

## Návrhy na vylepšení 💡
- Timeline OVR podle sezón (sloupec `Ovr` do `TopElevenSeasonStats`)
- Srovnání hráče s průměrem kádru na stejné pozici
- Permalink na srovnání `/compare/{this}/{other}`

## Brainstorming poznámky
- Po sloučení se `/stats` by detailní tabulka byla druhá záložka této stránky
