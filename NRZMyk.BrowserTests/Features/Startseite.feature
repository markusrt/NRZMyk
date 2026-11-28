# language: de
Funktionalität: Startseite
  Die Startseite zeigt je nach Rolle andere Inhalte.

  @guest
  Szenario: Gast sieht den Hinweis auf den eingeschränkten Zugang
    Angenommen ich bin als "Guest" angemeldet
    Dann sehe ich den Text "Eingeschränkter Gastzugang!"
    Und sehe ich nicht den Text "Feste Monate der Einsendung"

  @user
  Szenario: Benutzer sieht die Einsendemonate ohne Statistik
    Angenommen ich bin als "User" angemeldet
    Dann sehe ich die Texte:
      | Text                        |
      | Feste Monate der Einsendung |
      | Testlabor Nord              |
    Und sehe ich nicht den Text "Letzte Datenerfassung"
    Und sehe ich nicht den Text "Eingeschränkter Gastzugang!"

  @superuser
  Szenario: Super-User sieht die Einsendemonate mit Statistik
    Angenommen ich bin als "SuperUser" angemeldet
    Dann sehe ich die Texte:
      | Text                        |
      | Feste Monate der Einsendung |
      | Letzte Datenerfassung       |
      | Nächstes Einsendefenster    |
