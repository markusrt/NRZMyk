# language: de
@admin
Funktionalität: Administration
  Ein Administrator sieht alle Benutzer mit ihren Rollen und ordnet sie einem Labor zu.
  Die Rollen kommen aus dem Identitätsanbieter (Microsoft Graph), die Zuordnung aus der Datenbank.

  Grundlage:
    Angenommen ich bin als "Admin" angemeldet
    Und ich die Seite "admin" öffne

  Szenario: Benutzerliste mit Rollen
    Dann zeigt die Benutzerliste folgende Rollen:
      | Testnutzer | Rolle     |
      | Guest      | Guest     |
      | User       | Guest     |
      | SuperUser  | SuperUser |
      | Admin      | Admin     |

  Szenario: Gast einem Labor zuordnen
    Wenn ich dem Testnutzer "Guest" die Organisation "Testlabor Süd" zuordne
    Und ich die Seite "admin" öffne
    Dann ist dem Testnutzer "Guest" die Organisation "Testlabor Süd" zugeordnet
