# language: de
@user
Funktionalität: Sentinel Daten eines Labors
  Ein Benutzer sieht und bearbeitet nur die Einträge seines eigenen Labors ("Testlabor Nord", 57 Einträge).

  Grundlage:
    Angenommen ich bin als "User" angemeldet
    Und ich die Seite "sentinel-entries" öffne

  Szenario: Liste mit Blättern
    Dann sehe ich 50 Einträge
    Wenn ich auf "Nächste" klicke
    Dann sehe ich 7 Einträge

  Szenario: Suche nach der Labornummer des Einsenders
    Wenn ich nach "SUCHE-4711" suche
    Dann sehe ich 1 Eintrag
    Und sehe ich den Text "1 Einträge gefunden"

  Szenario: Einträge anderer Labore sind nicht sichtbar
    Wenn ich nach "SUED" suche
    Dann sehe ich 0 Einträge
    Und sehe ich den Text "0 Einträge gefunden"

  Szenario: Neuen Eintrag mit Validierungsfehlern und MHK-Eintrag anlegen
    Wenn ich auf "Neu anlegen" klicke
    Und ich auf "Anlegen" klicke
    Dann sehe ich die Texte:
      | Text                                             |
      | Das Feld Probenentnahme ist erforderlich         |
      | Das Feld Labornummer Einsender ist erforderlich  |
      | Das Feld Material ist erforderlich               |
      | Das Feld Spezies ist erforderlich                |
      | Mindestens ein MHK Eintrag ist erforderlich      |
    Wenn ich folgende Werte eingebe:
      | Feld                          | Wert                |
      | Material                      | Blutkultur peripher |
      | Geschlecht                    | weiblich            |
      | Probenentnahme                | 2026-11-01          |
      | Labornummer Einsender         | NEU-0001            |
      | Spezies                       | Candida albicans    |
      | Methode Speziesidentifikation | MALDI-TOF           |
    Und ich einen MHK-Eintrag "E-Test" für "Micafungin" mit dem Wert "0,12" hinzufüge
    Dann wird der MHK-Eintrag als "resistent" bewertet
    Wenn ich auf "Anlegen" klicke
    Und ich nach "NEU-0001" suche
    Dann sehe ich 1 Eintrag

  Szenario: Vorgänger aus einem anderen Labor wird nicht gefunden
    Wenn ich nach "NORD-0001" suche
    Und ich beim Eintrag "NORD-0001" auf "Eintrag bearbeiten" klicke
    Und ich folgende Werte eingebe:
      | Feld                     | Wert         |
      | Folgeisolat              | Ja           |
      | SN-Labornummer Vorgänger | SN-2025-0058 |
    Und ich auf "Speichern" klicke
    Dann sehe ich den Text "Die Labornummer wurde nicht gefunden"

  Szenario: Eintrag als Folgeisolat eines Vorgängers bearbeiten
    Wenn ich nach "NORD-0001" suche
    Und ich beim Eintrag "NORD-0001" auf "Eintrag bearbeiten" klicke
    Und ich folgende Werte eingebe:
      | Feld                     | Wert         |
      | Folgeisolat              | Ja           |
      | SN-Labornummer Vorgänger | SN-2025-0002 |
    Und ich auf "Speichern" klicke
    Und ich nach "NORD-0001" suche
    Und ich beim Eintrag "NORD-0001" auf "Eintrag bearbeiten" klicke
    Dann hat das Feld "SN-Labornummer Vorgänger" den Wert "SN-2025-0002"

  Szenario: Details eines bereits asservierten Eintrags ansehen
    Wenn ich nach "ARCHIV-0001" suche
    Und ich beim Eintrag "ARCHIV-0001" auf "Eintrag ansehen" klicke
    Dann sehe ich die Texte:
      | Text                                     |
      | Detailansicht Einsendung                 |
      | in einer Kryo-Box asserviert             |
      | Candida albicans                         |

  Szenario: Eintrag löschen
    Wenn ich nach "NORD-0003" suche
    Und ich beim Eintrag "NORD-0003" auf "Eintrag löschen" klicke
    Dann sehe ich den Text "Löschen bestätigen"
    Wenn ich auf "Eintrag löschen" klicke
    Dann sehe ich 0 Einträge
    Und sehe ich den Text "0 Einträge gefunden"
