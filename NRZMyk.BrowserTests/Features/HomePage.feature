Feature: Home page
  The home page shows different content depending on the role.

  @guest
  Scenario: Guest sees the notice about the restricted access
    Given I am signed in as "Guest"
    Then I see the text "Eingeschränkter Gastzugang!"
    And I do not see the text "Feste Monate der Einsendung"

  @user
  Scenario: User sees the submission months without statistics
    Given I am signed in as "User"
    Then I see the texts:
      | Text                        |
      | Feste Monate der Einsendung |
      | Testlabor Nord              |
    And I do not see the text "Letzte Datenerfassung"
    And I do not see the text "Eingeschränkter Gastzugang!"

  @superuser
  Scenario: Super user sees the submission months with statistics
    Given I am signed in as "SuperUser"
    Then I see the texts:
      | Text                        |
      | Feste Monate der Einsendung |
      | Letzte Datenerfassung       |
      | Nächstes Einsendefenster    |
