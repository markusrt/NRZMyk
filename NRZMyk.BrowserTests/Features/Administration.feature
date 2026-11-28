@admin
Feature: Administration
  An administrator sees all users with their roles and assigns them to a laboratory.
  The roles come from the identity provider (Microsoft Graph), the assignment from the database.

  Background:
    Given I am signed in as "Admin"
    And I open the page "admin"

  Scenario: User list with roles
    Then the user list shows the following roles:
      | Test user | Role      |
      | Guest     | Guest     |
      | User      | Guest     |
      | SuperUser | SuperUser |
      | Admin     | Admin     |

  Scenario: Assign a guest to a laboratory
    When I assign the organization "Testlabor Süd" to the test user "Guest"
    And I open the page "admin"
    Then the test user "Guest" is assigned the organization "Testlabor Süd"
