using SharedServices.Models.Achievement;

namespace TopElevenStats.Web.Achievements;

public static class TopElevenAchievements
{
    public static readonly IReadOnlyList<AchievementDef> All = new List<AchievementDef>
    {
        // ── Začátky (10) ──────────────────────────────────────────────────────
        new("FIRST_ACCOUNT",    "První účet",          "Vytvoř svůj první Top Eleven účet",                  "bi-person-circle",       10, "Začátky"),
        new("FIRST_PLAYER",     "První hráč",          "Přidej prvního hráče do záznamu",                    "bi-person-plus",         10, "Začátky"),
        new("FIRST_STATS",      "První statistiky",    "Zadej statistiky pro první sezónu",                  "bi-clipboard-data",      10, "Začátky"),
        new("FIRST_SEASON",     "Nová sezóna",         "Přidej svoji první sezónu",                          "bi-calendar-plus",       10, "Začátky"),
        new("FIRST_COMPARE",    "Srovnání",            "Porovnej dva hráče poprvé",                          "bi-people",              15, "Začátky"),
        new("FIRST_EXPORT",     "Export dat",          "Exportuj hráče do CSV poprvé",                       "bi-download",            15, "Začátky"),
        new("FIRST_IMPORT",     "Import dat",          "Importuj hráče z CSV poprvé",                        "bi-upload",              15, "Začátky"),
        new("FIRST_ELITE",      "Elitní hráč",         "Přidej prvního elitního hráče",                      "bi-star-fill",           20, "Začátky"),
        new("FIRST_GK",         "Brankář",             "Přidej prvního brankáře",                            "bi-shield",              15, "Začátky"),
        new("FIRST_VISIT_HOME", "Domů",                "Navštiv hlavní dashboard",                           "bi-house",                5, "Začátky"),

        // ── Hráči (20) ────────────────────────────────────────────────────────
        new("PLAYERS_5",        "Pět hráčů",           "Měj 5 hráčů v záznamu",                              "bi-people",              10, "Hráči"),
        new("PLAYERS_10",       "Deset hráčů",         "Měj 10 hráčů v záznamu",                             "bi-people-fill",         15, "Hráči"),
        new("PLAYERS_25",       "Čtvrtina sta",        "Měj 25 hráčů v záznamu",                             "bi-person-lines-fill",   20, "Hráči"),
        new("PLAYERS_50",       "Půl stovky",          "Měj 50 hráčů v záznamu",                             "bi-people",              30, "Hráči"),
        new("PLAYERS_100",      "Stovka!",             "Měj 100 hráčů v záznamu",                            "bi-trophy",              50, "Hráči"),
        new("PLAYERS_200",      "Dvě stě",             "Měj 200 hráčů v záznamu",                            "bi-trophy-fill",         75, "Hráči"),
        new("PLAYERS_500",      "Pět set!",            "Měj 500 hráčů v záznamu",                            "bi-award",              100, "Hráči"),
        new("DELETE_PLAYER",    "Smazaný hráč",        "Smaž prvního hráče ze záznamu",                      "bi-trash",                5, "Hráči"),
        new("EDIT_PLAYER",      "Úprava hráče",        "Uprav záznam hráče poprvé",                          "bi-pencil",               5, "Hráči"),
        new("PAGINATE",         "Listování",           "Přejdi na druhou stránku hráčů",                     "bi-chevron-right",        5, "Hráči"),
        new("SEARCH_PLAYERS",   "Hledač",              "Použij vyhledávání hráčů",                           "bi-search",               5, "Hráči"),
        new("FILTER_ELITE",     "Filtr elitní",        "Filtruj elitní hráče",                               "bi-funnel",               5, "Hráči"),
        new("FILTER_GK",        "Filtr brankáři",      "Filtruj brankáře",                                   "bi-funnel-fill",          5, "Hráči"),
        new("EXPORT_5_TIMES",   "Sběratel dat",        "Exportuj CSV 5krát",                                 "bi-collection",          20, "Hráči"),
        new("IMPORT_5_TIMES",   "Importér",            "Importuj CSV 5krát",                                 "bi-box-arrow-in-down",   20, "Hráči"),
        new("NO_RESULTS",       "Prázdno",             "Vyhledáváním nenajdi žádného hráče",                 "bi-emoji-frown",          5, "Hráči"),
        new("SAVE_PLAYER_10",   "Produktivní",         "Ulož 10 hráčů celkem",                               "bi-floppy",              20, "Hráči"),
        new("SAVE_PLAYER_50",   "Velmi produktivní",   "Ulož 50 hráčů celkem",                               "bi-floppy-fill",         40, "Hráči"),
        new("CITIZENSHIP_CZE",  "Češi",                "Přidej hráče s občanstvím CZE",                      "bi-flag",                10, "Hráči"),
        new("MULTI_ROLE",       "Vícepolohový",        "Přidej hráče s více rolemi (čárka v roli)",          "bi-grid",                10, "Hráči"),

        // ── OVR (15) ──────────────────────────────────────────────────────────
        new("OVR_70",           "Průměrný tým",        "Měj hráče s OVR 70+",                                "bi-graph-up",            10, "OVR"),
        new("OVR_75",           "Slušný tým",          "Měj hráče s OVR 75+",                                "bi-graph-up-arrow",      15, "OVR"),
        new("OVR_80",           "Dobrý tým",           "Měj hráče s OVR 80+",                                "bi-bar-chart",           20, "OVR"),
        new("OVR_85",           "Výborný tým",         "Měj hráče s OVR 85+",                                "bi-bar-chart-fill",      30, "OVR"),
        new("OVR_90",           "Skvělý tým",          "Měj hráče s OVR 90+",                                "bi-lightning",           50, "OVR"),
        new("OVR_95",           "Téměř perfektní",     "Měj hráče s OVR 95+",                                "bi-lightning-fill",      75, "OVR"),
        new("OVR_100",          "Dokonalý",            "Měj hráče s OVR 100",                                "bi-award-fill",         100, "OVR"),
        new("AVG_OVR_75",       "Průměr 75",           "Průměrné OVR všech hráčů dosáhne 75",                "bi-activity",            30, "OVR"),
        new("AVG_OVR_80",       "Průměr 80",           "Průměrné OVR všech hráčů dosáhne 80",                "bi-activity",            50, "OVR"),
        new("TOP5_OVR_80",      "Top 5 nad 80",        "Měj 5 hráčů s OVR 80+",                              "bi-list-ol",             25, "OVR"),
        new("TOP5_OVR_85",      "Top 5 nad 85",        "Měj 5 hráčů s OVR 85+",                              "bi-list-stars",          40, "OVR"),
        new("TOP10_OVR_80",     "Top 10 nad 80",       "Měj 10 hráčů s OVR 80+",                             "bi-1-circle",            50, "OVR"),
        new("GK_OVR_80",        "Brankář 80",          "Měj brankáře s OVR 80+",                             "bi-shield-check",        25, "OVR"),
        new("GK_OVR_85",        "Elitní brankář",      "Měj brankáře s OVR 85+",                             "bi-shield-fill-check",   40, "OVR"),
        new("MOST_OVR_PLAYER",  "Nejlepší hráč",       "Zobraz detail nejlepšího hráče",                     "bi-star",                10, "OVR"),

        // ── Elite (10) ────────────────────────────────────────────────────────
        new("ELITE_5",          "Pět elitních",        "Měj 5 elitních hráčů",                               "bi-stars",               20, "Elite"),
        new("ELITE_10",         "Deset elitních",      "Měj 10 elitních hráčů",                              "bi-star-fill",           35, "Elite"),
        new("ELITE_25",         "Elitní klub",         "Měj 25 elitních hráčů",                              "bi-trophy",              60, "Elite"),
        new("ELITE_50",         "Elitní armáda",       "Měj 50 elitních hráčů",                              "bi-trophy-fill",        100, "Elite"),
        new("ALL_ELITE",        "Plná elita",          "Měj všechny hráče elitní (min. 10)",                 "bi-award",              150, "Elite"),
        new("ELITE_GK",         "Elitní brankář",      "Přidej elitního brankáře",                           "bi-shield-fill",         30, "Elite"),
        new("ELITE_RATE_50",    "Půlka elitní",        "50% hráčů je elitní (min. 20 hráčů)",               "bi-percent",             40, "Elite"),
        new("ELITE_CZECH",      "Česká elita",         "Přidej elitního hráče s CZE občanstvím",             "bi-flag-fill",           20, "Elite"),
        new("REMOVE_ELITE",     "Degradace",           "Odeber elitu hráči",                                 "bi-star",                 5, "Elite"),
        new("ELITE_SPECIAL",    "Speciální schopnost", "Přidej elitního hráče se spec. schopností",          "bi-magic",               25, "Elite"),

        // ── Brankáři (8) ─────────────────────────────────────────────────────
        new("GK_3",             "Tři brankáři",        "Měj 3 brankáře",                                     "bi-shield",              15, "Brankáři"),
        new("GK_5",             "Pět brankářů",        "Měj 5 brankářů",                                     "bi-shield-fill",         25, "Brankáři"),
        new("GK_10",            "Deset brankářů",      "Měj 10 brankářů",                                    "bi-shield-shaded",       40, "Brankáři"),
        new("GK_YOUNG",         "Mladý brankář",       "Přidej brankáře mladšího než 22 let",                "bi-person-badge",        15, "Brankáři"),
        new("GK_VETERAN",       "Veterán",             "Přidej brankáře staršího než 35 let",                "bi-person-vcard",        15, "Brankáři"),
        new("GK_NO_ABILITY",    "Spolehlivý",          "Přidej brankáře bez spec. schopnosti",               "bi-shield-x",             5, "Brankáři"),
        new("GK_ABILITY",       "Specialista",         "Přidej brankáře se spec. schopností",                "bi-shield-plus",         15, "Brankáři"),
        new("GK_ELITE_MULTIPLE","Elitní obrana",       "Měj 3 elitní brankáře",                              "bi-shield-fill-check",   50, "Brankáři"),

        // ── Statistiky (15) ───────────────────────────────────────────────────
        new("STATS_5",          "Pět sezón",           "Zadej statistiky pro 5 sezón",                       "bi-calendar3",           15, "Statistiky"),
        new("STATS_10",         "Deset sezón",         "Zadej statistiky pro 10 sezón",                      "bi-calendar-check",      25, "Statistiky"),
        new("STATS_25",         "Série",               "Zadej statistiky pro 25 sezón",                      "bi-calendar-fill",       50, "Statistiky"),
        new("STATS_GOAL",       "Gólman",              "Zadej hráče s gólem v sezóně",                       "bi-circle",              10, "Statistiky"),
        new("STATS_ASSIST",     "Asistent",            "Zadej hráče s asistencí",                            "bi-hand-index",          10, "Statistiky"),
        new("STATS_RATING_9",   "Hvězdný výkon",       "Zadej hodnocení 9.0+",                               "bi-star-half",           20, "Statistiky"),
        new("STATS_CARD",       "Červená",             "Zadej červenou kartu",                               "bi-suit-hearts-fill",    10, "Statistiky"),
        new("STATS_CLEAN_SHEET","Nula",                "Zadej čisté konto",                                  "bi-0-circle",            15, "Statistiky"),
        new("STATS_WIN",        "Výhra",               "Zadej výhru s hráčem",                               "bi-check-circle",        10, "Statistiky"),
        new("STATS_LOSS",       "Prohra",              "Zadej prohru s hráčem",                              "bi-x-circle",            10, "Statistiky"),
        new("STATS_DRAW",       "Remíza",              "Zadej remízu s hráčem",                              "bi-dash-circle",          5, "Statistiky"),
        new("STATS_EXPORT",     "Statistiky ven",      "Exportuj statistiky do CSV",                         "bi-file-earmark-spreadsheet", 15, "Statistiky"),
        new("HIGH_WINRATE",     "Winning streak",      "Hráč má win rate nad 70%",                           "bi-graph-up-arrow",      25, "Statistiky"),
        new("STATS_100_GAMES",  "Stovka zápasů",       "Hráč odehraje 100 zápasů celkem",                   "bi-calendar-week",       50, "Statistiky"),
        new("STATS_PERFECT_SEASON","Perfektní sezóna", "Zadej sezónu bez prohry (min. 10 zápasů)",          "bi-gem",                 75, "Statistiky"),

        // ── Sezóny (10) ───────────────────────────────────────────────────────
        new("SEASON_3",         "Třetí sezóna",        "Zaznamenej 3 sezóny",                                "bi-3-circle",            20, "Sezóny"),
        new("SEASON_5",         "Pět sezón",           "Zaznamenej 5 sezón",                                 "bi-5-circle",            35, "Sezóny"),
        new("SEASON_10",        "Dekáda",              "Zaznamenej 10 sezón",                                "bi-1-circle-fill",       60, "Sezóny"),
        new("SEASON_20",        "Dvacet sezón",        "Zaznamenej 20 sezón",                                "bi-calendar4-week",     100, "Sezóny"),
        new("SEASON_RESET",     "Nový začátek",        "Přejdi na novou sezónu",                             "bi-arrow-clockwise",     10, "Sezóny"),
        new("COMPARE_SEASONS",  "Porovnání sezón",     "Porovnej hráče přes více sezón",                    "bi-arrows-expand",       20, "Sezóny"),
        new("BEST_SEASON",      "Nejlepší sezóna",     "Najdi sezónu s nejlepšími statistikami",             "bi-trophy",              25, "Sezóny"),
        new("ALL_STATS_IN",     "Kompletní",           "Zadej statistiky pro všechny hráče v sezóně (min. 5)", "bi-clipboard-check",  40, "Sezóny"),
        new("SEASON_MVP",       "MVP",                 "Označ hráče sezóny",                                 "bi-person-badge-fill",   30, "Sezóny"),
        new("LONG_CAREER",      "Dlouhá kariéra",      "Hráč má záznamy ve více než 5 sezónách",            "bi-hourglass",           40, "Sezóny"),

        // ── Export/Import (5) ─────────────────────────────────────────────────
        new("EXPORT_CSV",       "CSV export",          "Exportuj hráče do CSV",                              "bi-filetype-csv",        10, "Export/Import"),
        new("IMPORT_CSV",       "CSV import",          "Importuj hráče z CSV",                               "bi-file-earmark-arrow-up", 15, "Export/Import"),
        new("IMPORT_50",        "Hromadný import",     "Importuj 50+ hráčů naráz z CSV",                    "bi-cloud-upload",        30, "Export/Import"),
        new("EXPORT_STATS",     "Export statistik",    "Exportuj statistiky",                                "bi-file-earmark-bar-graph", 15, "Export/Import"),
        new("PERFECT_IMPORT",   "Bez chyb",            "Importuj CSV bez jediné chybové řady",              "bi-check-all",           25, "Export/Import"),

        // ── Speciální (7) ─────────────────────────────────────────────────────
        new("DARK_MODE",        "Temný mód",           "Přepni na tmavý režim",                              "bi-moon-fill",           10, "Speciální"),
        new("LIGHT_MODE",       "Světlý mód",          "Přepni na světlý režim",                             "bi-sun-fill",            10, "Speciální"),
        new("ALL_ACHIEVEMENTS", "Sběratel",            "Odemkni 50 achievementů",                            "bi-collection-fill",    200, "Speciální"),
        new("TOTAL_1000_PTS",   "Tisíc bodů",          "Získej 1000 bodů z achievementů",                   "bi-1-circle-fill",      150, "Speciální"),
        new("EARLY_BIRD",       "Ranní ptáče",         "Použij aplikaci v méně než 7 hodin ráno",           "bi-sunrise",             15, "Speciální"),
        new("NIGHT_OWL",        "Noční sova",          "Použij aplikaci po půlnoci",                         "bi-moon-stars",          15, "Speciální"),
        new("SPEED_RUN",        "Rychlý start",        "Přidej 5 hráčů během 5 minut",                      "bi-stopwatch",           25, "Speciální"),
    };
}
