@superuser
Feature: Coordinating laboratory
  A super user sees the entries of all laboratories, stores samples in cryo boxes and exports the data.

  Background:
    Given I am signed in as "SuperUser"

  Scenario: Entries of all laboratories are visible
    When I open the page "sentinel-entries"
    And I search for "SUED"
    Then I see 3 entries

  Scenario: Store a sample in the cryo view
    When I load the submissions of "Testlabor Nord" in the cryo view
    And I click "In Cryobox einlagern" for the entry "SN-2025-0056"
    Then the entry "SN-2025-0056" is stored in a cryo box today

  Scenario: Save a diagnostic finding as additional info
    When I load the submissions of "Testlabor Nord" in the cryo view
    And I save the additional info "Mischkultur, zusätzlich Candida glabrata nachgewiesen" for the entry "SN-2025-0057"
    And I load the submissions of "Testlabor Nord" in the cryo view
    Then the entry "SN-2025-0057" has the additional info "Mischkultur, zusätzlich Candida glabrata nachgewiesen"

  Scenario: Download the Excel export
    When I open the page "sentinel-entries"
    And I start the Excel export
    Then an Excel file is downloaded
