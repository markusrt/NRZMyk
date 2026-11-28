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
