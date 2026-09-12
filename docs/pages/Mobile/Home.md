# Mobile/Home.razor (MAUI)
Route: `/`
Popis: Úvod mobilní aplikace — počty hráčů, elitních, prům. OVR.
Platforma: Mobile (MAUI)

## Hotovo ✅
- 3 dlaždice se souhrnem, spinner načítání

## Chybí / Rozpracováno ⚠️
- **Žádné přihlášení ani filtr na účet** — čte všechny hráče celé DB
- Připojuje se přímo na PostgreSQL → connection string by musel být v aplikaci na telefonu; mimo LAN/Tailscale nefunguje
- `dotnet restore` Mobile projektu padá na NU1605 (downgrade EF Core 10.0.7 → 10.0.6)

## Návrhy na vylepšení 💡
- Přejít na HTTP API nad Web projektem (s autentizací) místo přímého DB přístupu

## Brainstorming poznámky
- Zvážit, zda MAUI udržovat, nebo nahradit PWA nad Blazorem
