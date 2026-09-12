# AchievementsPage.razor
Route: `/achievements`
Popis: Mřížka achievementů (`AchievementsGrid` ze SharedServices) s celkovým počtem bodů.
Platforma: Web

## Hotovo ✅
- Zobrazení všech 100 definic s odemčenými/zamčenými stavy a body

## Chybí / Rozpracováno ⚠️
- **Stav není per-user:** `AchievementService` ukládá do jednoho souboru `achievements.json` v ContentRoot — co odemkne jeden uživatel, mají všichni. MAB má hotové řešení `DbAchievementStore` (commit 9283c91)
- ~~62 ze 100 nedosažitelných~~ — 2026-09-12 doplněn `TopElevenAchievementEvaluator` (dopočet ze stavu DB) + triggery na stránkách. Zbývají ty, které potřebují chybějící funkci nebo perzistentní čítač: `EDIT_PLAYER`, `REMOVE_ELITE` (chybí editace hráče), `SEASON_MVP` (chybí označení MVP), `DARK_MODE`/`LIGHT_MODE` (hook v ThemePickeru ve SharedServices), `EXPORT_5_TIMES`, `IMPORT_5_TIMES`, `SAVE_PLAYER_10/50`, `SPEED_RUN` (čítače akcí)
- Chybí filtr podle kategorie a přepínač odemčené/zamčené

## Návrhy na vylepšení 💡
- Ukazatel postupu u počítaných achievementů („7/10 elitních“)

## Brainstorming poznámky
- Soubor v ContentRoot se navíc v Dockeru ztratí při každém redeployi (není ve volume)
