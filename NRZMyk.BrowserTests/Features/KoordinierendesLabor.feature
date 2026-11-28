# language: de
@superuser
Funktionalität: Koordinierendes Labor
  Ein Super-User sieht die Einträge aller Labore, asserviert Proben und exportiert die Daten.

  Grundlage:
    Angenommen ich bin als "SuperUser" angemeldet

  Szenario: Einträge aller Labore sind sichtbar
    Wenn ich die Seite "sentinel-entries" öffne
    Und ich nach "SUED" suche
    Dann sehe ich 3 Einträge

  Szenario: Probe in der Kryo-Ansicht asservieren
    Wenn ich in der Kryo-Ansicht die Einsendungen von "Testlabor Nord" lade
    Und ich beim Eintrag "SN-2025-0056" auf "In Cryobox einlagern" klicke
    Dann ist der Eintrag "SN-2025-0056" heute asserviert

  Szenario: Diagnostische Besonderheit als zusätzliche Info speichern
    Wenn ich in der Kryo-Ansicht die Einsendungen von "Testlabor Nord" lade
    Und ich beim Eintrag "SN-2025-0057" die zusätzliche Info "Mischkultur, zusätzlich Candida glabrata nachgewiesen" speichere
    Und ich in der Kryo-Ansicht die Einsendungen von "Testlabor Nord" lade
    Dann hat der Eintrag "SN-2025-0057" die zusätzliche Info "Mischkultur, zusätzlich Candida glabrata nachgewiesen"

  Szenario: Excel-Export herunterladen
    Wenn ich die Seite "sentinel-entries" öffne
    Und ich den Excel-Export starte
    Dann wird eine Excel-Datei heruntergeladen
