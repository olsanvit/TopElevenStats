# Players.razor
Route: `/players`
Popis: Správa kádru — založení účtu, přidání hráče, seznam se stránkováním, filtry, export CSV, mazání.
Platforma: Web (mobilní obdoba: Mobile/PlayersPage.razor)

## Hotovo ✅
- Založení Top Eleven účtu (když uživatel žádný nemá)
- Inline formulář nového hráče (jméno, OVR, věk, občanství, role, spec. schopnost, GK, elitní)
- Server-side vyhledávání jménem + filtry elitní/brankář, stránkování po 20 (`Paginator`)
- Export CSV, mazání přes `ConfirmDialog`
- ~25 achievement checků při přidání hráče
- **2026-09-11:** účet přes `TopElevenAccountAccessor`, nový účet dostane `OwnerUserId` přihlášeného uživatele
- **2026-09-12:** `PAGINATE` + `NO_RESULTS` doplněny; po změně kádru běží `TopElevenAchievementEvaluator`
- **2026-09-11:** mazání hráče ověřuje `AccountId` v dotazu (ne jen v UI), `FIRST_ACCOUNT` achievement se konečně odemyká

## Chybí / Rozpracováno ⚠️
- **Nejde editovat hráče** — OVR a věk se v Top Eleven mění každou sezónu (achievement `EDIT_PLAYER` nedosažitelný)
- `TopElevenPlayerValidator` je registrovaný v DI, ale nepoužívá se (jen ruční kontrola jména)
- Přidání nekontroluje duplicitu jména
- Řazení natvrdo podle OVR; chybí filtr podle role, rozsahu věku a OVR
- `SAVE_PLAYER_10/50`, `SPEED_RUN`, `EXPORT_5_TIMES` vyžadují perzistentní čítače akcí (zatím nevolané)

## Návrhy na vylepšení 💡
- Inline editace v řádku (rychlé OVR +1 na začátku sezóny)
- Hromadné akce: označit víc hráčů → smazat / nastavit elitní
- Sloupec „poslední hodnocení“ s trendovou šipkou, badge „bez statistik“

## Brainstorming poznámky
- Formulář přejít na `EditForm` + `FluentValidationValidator` — validátor už existuje
- Bulk „stárnutí kádru +1 rok“ patří spíš do přechodu sezóny (Season.razor)
