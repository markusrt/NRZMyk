# language: de
Funktionalität: Anmeldung
  Geschützte Seiten sind nur nach der Anmeldung erreichbar.
  Jede Rolle meldet sich über die echte Anmeldeseite an und sieht nur ihre Menüpunkte.

  @anonym
  Szenario: Geschützte Seite leitet ohne Anmeldung zur Anmeldeseite weiter
    Wenn ich ohne Anmeldung die Seite "sentinel-entries" öffne
    Dann werde ich zur Anmeldeseite weitergeleitet

  Szenariogrundriss: Anmeldung als <Rolle>
    Wenn ich mich als "<Rolle>" anmelde
    Dann bin ich angemeldet
    Und sehe ich im Menü "<Menüpunkt>"
    Aber nicht "<ohne Menüpunkt>"

    @guest
    Beispiele: Gast
      | Rolle | Menüpunkt  | ohne Menüpunkt |
      | Guest | Startseite | Sentinel Daten |

    @user
    Beispiele: Benutzer
      | Rolle | Menüpunkt      | ohne Menüpunkt    |
      | User  | Sentinel Daten | Kryo Asservierung |

    @superuser
    Beispiele: Super-User
      | Rolle     | Menüpunkt         | ohne Menüpunkt |
      | SuperUser | Kryo Asservierung | Administration |

    @admin
    Beispiele: Administrator
      | Rolle | Menüpunkt      | ohne Menüpunkt    |
      | Admin | Administration | Kryo Asservierung |

  @user
  Szenario: Abmelden
    Wenn ich mich als "User" anmelde
    Und ich mich abmelde
    Dann bin ich abgemeldet
    Und sehe ich im Menü nicht "Sentinel Daten"

  @user @admin
  Szenario: Freischaltung eines neuen Benutzers durch Zuordnung zu einem Labor
    Angenommen der Testnutzer "User" hat sich noch nie angemeldet
    Wenn ich mich als "User" anmelde
    Dann sehe ich den Text "Eingeschränkter Gastzugang!"
    Und sehe ich im Menü nicht "Sentinel Daten"
    Wenn ich in einem neuen Browserfenster als "Admin" angemeldet bin
    Und ich die Seite "admin" öffne
    Und ich dem Testnutzer "User" die Organisation "Testlabor Nord" zuordne
    Und ich in einem neuen Browserfenster als "User" angemeldet bin
    Dann sehe ich im Menü "Sentinel Daten"
    Und sehe ich nicht den Text "Eingeschränkter Gastzugang!"
