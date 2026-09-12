# AdminDashboard.razor
Route: `/admin` — `[Authorize(Roles = "Admin")]`
Popis: Admin přehled — počítadla, seznam uživatelů, správa Top Eleven účtů (±sezóna, mazání), top 10 hráčů podle OVR.
Platforma: Web

## Hotovo ✅
- Počítadla uživatelů, účtů, hráčů, sezónních záznamů
- Seznam uživatelů s rolí, správa sezóny a mazání účtů (s kaskádou)
- Top 10 hráčů napříč účty
- **2026-09-12:** počty hráčů/záznamů jedním `GroupBy` dotazem místo N+1 synchronních Count() na SignalR vlákně
- **2026-09-11:** sloupec „Vlastník“ u účtů; účet bez vlastníka má červený badge

## Chybí / Rozpracováno ⚠️
- Nelze přidělit/odebrat roli Admin, jen se zobrazí
- Nelze převést účet na jiného vlastníka
- Chybí zdraví aplikace: poslední chyby ze Serilog tabulky `Logs`, velikost DB

## Návrhy na vylepšení 💡
- Počty přes jeden `GroupBy` dotaz
- Správa uživatelů (vzor: MAB `UsersAdmin.razor`)
- Akce „převést účet na uživatele…“

## Brainstorming poznámky
- Admin záměrně vidí data všech účtů — jediná stránka, kde filtr na vlastníka být nemá
