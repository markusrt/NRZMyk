@user
Feature: Sentinel entries of a laboratory
  A user only sees and edits the entries of their own laboratory ("Testlabor Nord", 57 entries).

  Background:
    Given I am signed in as "User"
    And I open the page "sentinel-entries"

  Scenario: List with paging
    Then I see 50 entries
    When I click "Nächste"
    Then I see 7 entries

  Scenario: Search by the sender's laboratory number
    When I search for "SUCHE-4711"
    Then I see 1 entry
    And I see the text "1 Einträge gefunden"

  Scenario: Entries of other laboratories are not visible
    When I search for "SUED"
    Then I see 0 entries
    And I see the text "0 Einträge gefunden"

  Scenario: Create a new entry with validation errors and an MIC entry
    When I click "Neu anlegen"
    And I click "Anlegen"
    Then I see the texts:
      | Text                                            |
      | Das Feld Probenentnahme ist erforderlich        |
      | Das Feld Labornummer Einsender ist erforderlich |
      | Das Feld Material ist erforderlich              |
      | Das Feld Spezies ist erforderlich               |
      | Mindestens ein MHK Eintrag ist erforderlich     |
    When I enter the following values:
      | Field                         | Value               |
      | Material                      | Blutkultur peripher |
      | Geschlecht                    | weiblich            |
      | Probenentnahme                | 2026-11-01          |
      | Labornummer Einsender         | NEU-0001            |
      | Spezies                       | Candida albicans    |
      | Methode Speziesidentifikation | MALDI-TOF           |
    And I add an MIC entry "E-Test" for "Micafungin" with the value "0,12"
    Then the MIC entry is rated "resistent"
    When I click "Anlegen"
    And I search for "NEU-0001"
    Then I see 1 entry

  Scenario: Predecessor from another laboratory is not found
    When I search for "NORD-0001"
    And I click "Eintrag bearbeiten" for the entry "NORD-0001"
    And I enter the following values:
      | Field                    | Value        |
      | Folgeisolat              | Ja           |
      | SN-Labornummer Vorgänger | SN-2025-0058 |
    And I click "Speichern"
    Then I see the text "Die Labornummer wurde nicht gefunden"

  Scenario: Edit an entry as a follow-up isolate of a predecessor
    When I search for "NORD-0001"
    And I click "Eintrag bearbeiten" for the entry "NORD-0001"
    And I enter the following values:
      | Field                    | Value        |
      | Folgeisolat              | Ja           |
      | SN-Labornummer Vorgänger | SN-2025-0002 |
    And I click "Speichern"
    And I search for "NORD-0001"
    And I click "Eintrag bearbeiten" for the entry "NORD-0001"
    Then the field "SN-Labornummer Vorgänger" has the value "SN-2025-0002"

  Scenario: View the details of an entry already stored in a cryo box
    When I search for "ARCHIV-0001"
    And I click "Eintrag ansehen" for the entry "ARCHIV-0001"
    Then I see the texts:
      | Text                         |
      | Detailansicht Einsendung     |
      | in einer Kryo-Box asserviert |
      | Candida albicans             |

  Scenario: Delete an entry
    When I search for "NORD-0003"
    And I click "Eintrag löschen" for the entry "NORD-0003"
    Then I see the text "Löschen bestätigen"
    When I click "Eintrag löschen"
    Then I see 0 entries
    And I see the text "0 Einträge gefunden"
