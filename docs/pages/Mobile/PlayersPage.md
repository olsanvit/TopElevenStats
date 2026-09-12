# Mobile/PlayersPage.razor (MAUI)
Route: `/players`
Popis: Read-only seznam hráčů s vyhledáváním, filtry elitní/brankář a stránkováním.
Platforma: Mobile (MAUI)

## Hotovo ✅
- Vyhledávání, filtry, stránkování, badge Elite/GK

## Chybí / Rozpracováno ⚠️
- Stejné jako Mobile/Home: bez auth, čte hráče všech účtů přímo z DB
- Žádný detail hráče, žádné statistiky

## Návrhy na vylepšení 💡
- Detail hráče se statistikami, po přechodu na API

## Brainstorming poznámky
- `ToLower().Contains()` místo `ILike` — funguje, ale neumí index
